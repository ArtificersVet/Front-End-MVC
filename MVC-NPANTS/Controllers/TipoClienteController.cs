using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;


namespace MVC_NPANTS.Controllers
{
    public class TipoClienteController : Controller
    {
        HttpClient _httpClient;

        public TipoClienteController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchString = null)
        {
            SetAuthorizationHeader();

            List<TipoCliente> tiposClientes = new List<TipoCliente>();
            int totalPages = 0;

            try
            {
                // Obtener todos los tipos de cliente desde el endpoint con paginación
                var response = await _httpClient.GetFromJsonAsync<PagedTipoClienteResponse>($"tipoclientes?page={page}&pageSize={pageSize}");

                if (response != null)
                {
                    tiposClientes = response.TipoClientes;
                    totalPages = response.TotalPages;
                }

                // Filtrar por el término de búsqueda si existe
                if (!string.IsNullOrEmpty(searchString))
                {
                    tiposClientes = tiposClientes.Where(tc =>
                        tc.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                    ViewBag.SearchString = searchString; // Guardar el término en ViewBag para mostrarlo en la vista
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener la lista de tipos de cliente: " + ex.Message);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Crear el modelo de vista
            var viewModel = new PagedTipoClienteResponse
            {
                TipoClientes = tiposClientes,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }





        public ActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

       
        [HttpPost]
        public async Task <IActionResult> Create(TipoCliente TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PostAsJsonAsync("tipoclientes/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        

        
        public async Task <IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo cliente");

            return View(tipo);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoCliente TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PutAsJsonAsync($"tipoclientes/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }


        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        
        public async Task <IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.DeleteAsync($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se elimino");

            return RedirectToAction(nameof(Index));
        }

       
    }
}
