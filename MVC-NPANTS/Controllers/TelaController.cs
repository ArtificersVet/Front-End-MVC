using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Net.Http.Json;

namespace MVC_NPANTS.Controllers
{
    public class TelaController : Controller
    {
        private readonly HttpClient _httpClient;

        public TelaController(IHttpClientFactory httpClientFactory)
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

        // Listar todas las telas
        public async Task<IActionResult> Index(int page = 1, string searchString = null, int pageSize = 10)
        {
            SetAuthorizationHeader();

            List<Tela> telas = new List<Tela>();
            int totalPages = 0;
            try
            {
                var pagedResponse = await _httpClient.GetFromJsonAsync<PageTelaResponse>($"telas?page={page}");

                if (pagedResponse != null)
                {
                    telas = pagedResponse.Telas;
                    totalPages = pagedResponse.TotalPages;
                }

                // Filtrar por el término de búsqueda si existe
                if (!string.IsNullOrEmpty(searchString))
                {
                    telas = telas.Where(t =>
                         t.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        t.Color.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                    ViewBag.SearchString = searchString; // Guardar el término en ViewBag para mostrarlo en la vista
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener la lista de telas: " + ex.Message);
            }
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            var viewModel = new PageTelaResponse
            {
                Telas = telas,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // Mostrar el formulario para crear una nueva tela
        public ActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

        // Procesar el formulario de creación de tela
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tela tela)
        {

            SetAuthorizationHeader();
            var tipo = await _httpClient.PostAsJsonAsync("telas/create", tela);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // Obtener los detalles de una tela por ID
        public async Task<IActionResult> Details(long id)
        {
            SetAuthorizationHeader();
            var tela = await _httpClient.GetFromJsonAsync<Tela>($"telas/{id}");

            if (tela == null)
            {
                Console.WriteLine($"No se encontró la tela con ID {id}");
            }

            return View(tela);
        }

        // Mostrar el formulario de edición de una tela
        public async Task<IActionResult> Edit(long id)
        {
            SetAuthorizationHeader();
            var tela = await _httpClient.GetFromJsonAsync<Tela>($"telas/{id}");

            if (tela == null)
            {
                Console.WriteLine("No se encontró la tela");
            }

            return View(tela);
        }

        // Procesar el formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Tela tela)
        {
            if (id != tela.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                SetAuthorizationHeader();
                var result = await _httpClient.PutAsJsonAsync($"telas/{id}", tela);

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(tela);
        }
        // Mostrar la vista de confirmación de eliminación
        public async Task<IActionResult> Delete(long id)
        {
            SetAuthorizationHeader();
            var tela = await _httpClient.GetFromJsonAsync<Tela>($"telas/{id}");

            if (tela == null)
            {
                return NotFound();
            }

            return View(tela);
        }


        // Eliminar una tela
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();
            var result = await _httpClient.DeleteAsync($"telas/{id}");

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("No se pudo eliminar la tela");
            return RedirectToAction(nameof(Index));
        }

    }
}