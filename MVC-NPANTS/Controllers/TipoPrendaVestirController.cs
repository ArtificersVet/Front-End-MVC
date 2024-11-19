using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;


namespace MVC_NPANTS.Controllers
{
    public class TipoPrendaVestirController : Controller
    {
        HttpClient _httpClient;

        public TipoPrendaVestirController(IHttpClientFactory httpClientFactory)
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

            List<TipoPrendaVestir> tiposPrenda = new List<TipoPrendaVestir>();
            int totalPages = 0;

            try
            {
                // Obtener todos los tipos de prenda desde el endpoint con paginación
                var response = await _httpClient.GetFromJsonAsync<PaginacionTipoPrendas>($"tipoprendasvestir?page={page}&pageSize={pageSize}");

                if (response != null)
                {
                    tiposPrenda = response.TipoPrendas;
                    totalPages = response.TotalPages;
                }

                // Filtrar por el término de búsqueda si existe
                if (!string.IsNullOrEmpty(searchString))
                {
                    tiposPrenda = tiposPrenda.Where(tp =>
                        tp.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                    ViewBag.SearchString = searchString; // Guardar el término en ViewBag para mostrarlo en la vista
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener la lista de tipos de prenda: " + ex.Message);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Crear el modelo de vista
            var viewModel = new PaginacionTipoPrendas
            {
                TipoPrendas = tiposPrenda,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }


        // GET: TipoPrendaVestirController/Create
        public ActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

        // POST: TipoPrendaVestirController/Create
        [HttpPost]

        public async Task<IActionResult>  Create(TipoPrendaVestir TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PostAsJsonAsync("tipoprendasvestir/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo prenda");

            return View(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoPrendaVestir TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PutAsJsonAsync($"tipoprendasvestir/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.DeleteAsync($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se eliminó");

            return RedirectToAction(nameof(Index));
        }
    }
}
