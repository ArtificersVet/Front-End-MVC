using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JsonException = Newtonsoft.Json.JsonException;

namespace MVC_NPANTS.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(IHttpClientFactory httpClientFactory, ILogger<PedidoController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
            _logger = logger;
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            try
            {
                SetAuthorizationHeader();
                 var response = await _httpClient.GetAsync($"pedidos?page={page}&pageSize={pageSize}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                     return View(CreateEmptyPedidoResponse(pageSize));
                }

                response.EnsureSuccessStatusCode();
                 var pedidoResponse = await response.Content.ReadFromJsonAsync<PedidoResponse>();

                if (pedidoResponse == null)
                {
                    return View(CreateEmptyPedidoResponse(pageSize));
                }

                 return View(pedidoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pedidos");
                 return View(CreateEmptyPedidoResponse(pageSize));
            }
        }


        public async Task<IActionResult> Create()
        {
            try
            {
                SetAuthorizationHeader();

                // Cargar clientes
                var clientesResponse = await GetApiResponse<PagedClientesResponse>("clientes");
                ViewBag.Clientes = new SelectList(clientesResponse?.Clientes ?? new List<Cliente>(), "Id", "Nombre");

                // Cargar estados de pedido
                var estadosResponse = await GetApiResponse<List<EstadoPedido>>("estadosPedido");
                ViewBag.EstadoPedido = new SelectList(estadosResponse ?? new List<EstadoPedido>(), "Id", "Nombre");

                // Cargar prendas
                var prendasResponse = await GetApiResponse<PrendaVestirResponse>("prendas");
                ViewBag.PrendasVestir = new SelectList(prendasResponse?.prendaVestirs ?? new List<PrendaVestir>(), "Id", "Nombre");

                // Cargar tallas
                var tallasResponse = await GetApiResponse<TallasResponse>("tallas");
                ViewBag.Tallas = new SelectList(tallasResponse?.Data ?? new List<Talla>(), "Id", "Nombre");

                return View(new Pedido { FechaPedido = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create GET action");
                ModelState.AddModelError(string.Empty, "Error al cargar los datos necesarios para crear el pedido");
                return View(new Pedido { FechaPedido = DateTime.Now });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido, List<DetalleProducto> detalles)
        {
            try
            {
                SetAuthorizationHeader();

                var detallesValidos = ValidateAndFilterDetalles(detalles);
                if (!detallesValidos.Any())
                {
                    ModelState.AddModelError(string.Empty, "Debe incluir al menos un detalle de producto válido");
                    await LoadViewBags();
                    return View(pedido);
                }

                var pedidoData = CreatePedidoData(pedido, detallesValidos);
                var response = await _httpClient.PostAsJsonAsync("pedidos/create", pedidoData);

                if (response.IsSuccessStatusCode)
                {
                    // Obtener la respuesta de la API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Obtener el enlace de aprobación de PayPal
                    ViewData["ApprovalLink"] = responseData.approvalLink;

                    // Redirigir al usuario a la URL de aprobación de PayPal
                    return View(pedido);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al crear el pedido: {errorContent}");
                await LoadViewBags();
                return View(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pedido");
                ModelState.AddModelError(string.Empty, "Error al procesar la solicitud");
                await LoadViewBags();
                return View(pedido);
            }
        }

        public async Task<IActionResult> CapturePayment(string token, string PayerID)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"pedidos/capture-payment?token={token}&PayerID={PayerID}");

                if (response.IsSuccessStatusCode)
                {
                    // El pago se completó correctamente
                    return RedirectToAction("Index");
                }
                else
                {
                    // Hubo un error al procesar el pago
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al procesar el pago: {errorContent}");
                    return View("Error", new { message = "Error al procesar el pago" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing payment");
                return View("Error", new { message = "Error al procesar el pago" });
            }
        }

        private async Task LoadViewBags()
        {
            try
            {
                var clientesResponse = await GetApiResponse<PagedClientesResponse>("clientes");
                ViewBag.Clientes = clientesResponse?.Clientes?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }) ?? new List<SelectListItem>();

                var estadosResponse = await GetApiResponse<List<EstadoPedido>>("estadosPedido");
                ViewBag.EstadoPedido = estadosResponse?.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                }) ?? new List<SelectListItem>();

                var prendasResponse = await GetApiResponse<PrendaVestirResponse>("prendas");
                ViewBag.PrendasVestir = prendasResponse?.prendaVestirs?.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }) ?? new List<SelectListItem>();

                var tallasResponse = await GetApiResponse<TallasResponse>("tallas");
                ViewBag.Tallas = tallasResponse?.Data?.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nombre
                }) ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ViewBags");
                InitializeEmptyViewBags();
            }
        }


        private async Task<T> GetApiResponse<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON deserialization error for endpoint {endpoint}");
                return default;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request error for endpoint {endpoint}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error for endpoint {endpoint}");
                return default;
            }
        }


        private List<DetalleProducto> ValidateAndFilterDetalles(List<DetalleProducto> detalles)
        {
            return detalles?
                .Where(d => d != null
                    && d.PrendaVestirId > 0
                    && d.TallaId > 0
                    && d.Cantidad > 0
                    && d.Precio > 0
                    && d.TotalPieza > 0
                    && d.ConsumoTela != null
                    && d.SubTotal > 0)
                .ToList() ?? new List<DetalleProducto>();
        }

        private object CreatePedidoData(Pedido pedido, List<DetalleProducto> detalles)
        {
            return new
            {
                fecha_pedido = pedido.FechaPedido.ToString("yyyy-MM-dd"),
                saldo = pedido.Saldo,
                tipo_pago = pedido.TipoPago,
                total = pedido.Total,
                cliente_id = pedido.ClienteId,
                estado_pedido_id = pedido.EstadoPedidoId,
                detalleproducto = detalles.Select(d => new
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
        }

        private void InitializeEmptyViewBags()
        {
            ViewBag.Tallas = new List<Talla>();
            ViewBag.EstadosPedido = new List<EstadoPedido>();
            ViewBag.PrendasVestir = new List<PrendaVestir>();
            ViewBag.Clientes = new List<Cliente>();
        }

        private PedidoResponse CreateEmptyPedidoResponse(int pageSize)
        {
            return new PedidoResponse
            {
                Pedidos = new List<Pedido>(),
                CurrentPage = 1,
                PageSize = pageSize,
                TotalItems = 0,
                TotalPages = 1
            };
        }

        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                SetAuthorizationHeader();
                var pedido = await GetApiResponse<Pedido>($"pedidos/{id}");

                if (pedido == null)
                {
                    return NotFound();
                }

                return View(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving pedido {id} for deletion");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"pedidos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error deleting pedido {id}: {errorContent}");
                return View("Error", new { message = "Error al eliminar el pedido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting pedido {id}");
                return View("Error", new { message = "Error al eliminar el pedido" });
            }
        }

        public async Task<IActionResult> Details(long id)
        {
            try
            {
                SetAuthorizationHeader();
                var pedido = await GetApiResponse<Pedido>($"pedidos/{id}");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving pedido details {id}");
                return NotFound();
            }
        }
    }
}