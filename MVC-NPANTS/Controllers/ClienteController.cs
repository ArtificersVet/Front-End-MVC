using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class ClienteController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClienteController(IHttpClientFactory httpClientFactory)
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

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            SetAuthorizationHeader();

            var viewModel = new PagedClientesResponse();

            try
            {
                var pagedResponse = await _httpClient.GetFromJsonAsync<PagedClientesResponse>($"clientes?page={page}");

                if (pagedResponse != null)
                {
                    viewModel.Clientes = pagedResponse.Clientes;
                    viewModel.CurrentPage = pagedResponse.CurrentPage;
                    viewModel.TotalPages = pagedResponse.TotalPages;
                    viewModel.PageSize = pagedResponse.PageSize;
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Si el endpoint devuelve un 404, devolvemos la vista con una lista vacía en lugar de lanzar el error
                viewModel.Clientes = new List<Cliente>();
                viewModel.CurrentPage = 1;
                viewModel.TotalPages = 1;
                viewModel.PageSize = 10;
            }
            catch (Exception)
            {
                // Cualquier otro tipo de excepción se captura para evitar que la aplicación falle
                viewModel.Clientes = new List<Cliente>();
            }

            return View(viewModel);
        }

        // GET: ClienteController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");

            if (cliente == null)
            {
                return NotFound();
            }

            if (cliente.TipoclienteId.HasValue)
            {
                var tipoCliente = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{cliente.TipoclienteId.Value}");
                cliente.Tipocliente = tipoCliente;
            }

            return View(cliente);
        }

        // GET: ClienteController/Create
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");

            if (tiposClientes == null || !tiposClientes.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de clientes.");
                ViewBag.TiposClientes = new SelectList(Enumerable.Empty<TipoCliente>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");
            }

            return View();
        }


    // POST: ClienteController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        SetAuthorizationHeader();

        if (ModelState.IsValid)
        {
            if (cliente.TipoclienteId == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un tipo de cliente.");
            }
            else
            {
                var response = await _httpClient.PostAsJsonAsync("clientes/create", cliente);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo crear el cliente.");
            }
        }

        var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
        ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");

        return View(cliente);
    }


        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();

            // Cargar la lista de tipos de clientes
            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");

            if (tiposClientes == null || !tiposClientes.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de clientes.");
                ViewBag.TiposClientes = new SelectList(Enumerable.Empty<TipoCliente>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");
            }

            // Obtener el cliente a editar
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            SetAuthorizationHeader();

            if (ModelState.IsValid)
            {
                if (cliente.TipoclienteId == null)
                {
                    ModelState.AddModelError("", "Debe seleccionar un tipo de cliente.");
                }
                else
                {
                    var response = await _httpClient.PutAsJsonAsync($"clientes/{id}", cliente);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "No se pudo actualizar el cliente.");
                }
            }

            // Cargar la lista de tipos de clientes nuevamente si la actualización falla
            var tiposClientes = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");
            ViewBag.TiposClientes = new SelectList(tiposClientes, "Id", "Nombre");

            return View(cliente);
        }


        // GET: ClienteController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            SetAuthorizationHeader();

            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"clientes/{id}");

            if (cliente == null)
            {
                return NotFound();
            }
            if (cliente.TipoclienteId.HasValue)
            {
                var tipoCliente = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{cliente.TipoclienteId.Value}");
                cliente.Tipocliente = tipoCliente;
            }
            return View(cliente);

        }

        // POST: ClienteController/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"clientes/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo eliminar el cliente.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Error al eliminar el cliente.");
                return RedirectToAction(nameof(Index));
            }



        }
    }

    public class TipoClientesResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<TipoCliente> TipoClientes { get; set; }
    }

}
