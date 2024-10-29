﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MVC_NPANTS.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _httpClient;

        public PedidoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        
        public async Task<IActionResult> Index(int page = 1, int pageSize =12)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pedidos?page={page}&pageSize={pageSize}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return View(new PedidoResponse
                    {
                        Pedidos = new List<Pedido>(),
                        CurrentPage = 1,
                        PageSize = pageSize,
                        TotalItems = 0,
                        TotalPages = 1
                    });
                }

                response.EnsureSuccessStatusCode();

            

                var pedidoResponse = await response.Content.ReadFromJsonAsync<PedidoResponse>();
                return View(pedidoResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Devolver una respuesta vacía en caso de error
                return View(new PedidoResponse
                {
                    Pedidos = new List<Pedido>(),
                    CurrentPage = 1,
                    PageSize = pageSize,
                    TotalItems = 0,
                    TotalPages = 1
                });
            }
        }


        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();
            await LoadViewBags();

            var response = await _httpClient.GetFromJsonAsync<PagedClientesResponse>("clientes");
            var clientes = response?.Clientes;  
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre");  
            

            var responseEs = await _httpClient.GetFromJsonAsync<EstadoPedidosResponse>("estadosPedido");
            var estadosPedidos = responseEs?.estadoPedidos;
            ViewBag.EstadoPedido = new SelectList(estadosPedidos, "Id", "Nombre");

            var responsePrenda = await _httpClient.GetFromJsonAsync<PrendaVestirResponse>("prendas");
            var prendasVestir = responsePrenda?.prendaVestirs;
            ViewBag.PrendasVestir = new SelectList(prendasVestir, "Id", "Nombre");

            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");
            ViewBag.Tallas = new SelectList(tallas, "Id", "Nombre");

            return View(new Pedido { FechaPedido = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido, List<DetalleProducto> detalles)
        {
            SetAuthorizationHeader();

            // Filtrar detalles válidos
            var detallesValidos = detalles?
                .Where(d => d != null
                         && d.PrendaVestirId > 0
                         && d.TallaId > 0
                         && d.Cantidad > 0
                         && d.Precio > 0
                         && d.TotalPieza > 0
                         && d.ConsumoTela != null
                         && d.SubTotal > 0)
                .ToList() ?? new List<DetalleProducto>();

            // Comprobar si hay detalles válidos
            if (!detallesValidos.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe incluir al menos un detalle de producto válido");
                await LoadViewBags();
                return View(pedido);
            }

            var pedidoData = new
            {
                fecha_pedido = pedido.FechaPedido.ToString("yyyy-MM-dd"),
                saldo = pedido.Saldo,
                tipo_pago = pedido.TipoPago,
                total = pedido.Total,
                cliente_id = pedido.ClienteId,
                estado_pedido_id = pedido.EstadoPedidoId,
                detalleproducto = detallesValidos.Select(d => new
                {
                    prenda_vestir_id = d.PrendaVestirId,
                    talla_id = d.TallaId,
                    cantidad = d.Cantidad,
                    descripcion = d.Descripcion,
                    precio = d.Precio,
                    total_pieza = d.Cantidad * d.Precio,
                    consumo_tela = d.ConsumoTela ?? 0,
                    sub_total = d.Cantidad * d.Precio
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("pedidos/create", pedidoData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al crear el pedido: {errorContent}");
            }

            await LoadViewBags();
            return View(pedido);
        }


        private async Task LoadViewBags()
        {
            try
            {
                ViewBag.Tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas") ?? new List<Talla>();
                ViewBag.EstadosPedido = await _httpClient.GetFromJsonAsync<List<EstadoPedido>>("estadospedido") ?? new List<EstadoPedido>();
                ViewBag.PrendasVestir = await _httpClient.GetFromJsonAsync<List<PrendaVestir>>("prendas") ?? new List<PrendaVestir>();
                ViewBag.Clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("clientes") ?? new List<Cliente>();
            }
            catch (Exception)
            {
                ViewBag.Tallas = new List<Talla>();
                ViewBag.EstadosPedido = new List<EstadoPedido>();
                ViewBag.PrendasVestir = new List<PrendaVestir>();
                ViewBag.Clientes = new List<Cliente>();
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            SetAuthorizationHeader();
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"pedidos/{id}");

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"pedidos/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new { message = "Error al eliminar el pedido" });
            }
        }

        public async Task<IActionResult> Details(long id)
        {
            SetAuthorizationHeader();
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"pedidos/{id}");

            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.Detalles == null || !pedido.Detalles.Any())
            {
                ViewBag.MensajeError = "No se encontraron detalles del producto para este pedido.";
            }

            return View(pedido);
        }
    }

}