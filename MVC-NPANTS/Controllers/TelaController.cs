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
        public async Task<IActionResult> Index(string searchString)
        {
            SetAuthorizationHeader();

            var telas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");

            if (!string.IsNullOrEmpty(searchString))
            {
                // Apply search filter
                telas = telas.Where(s => s.Nombre.Contains(searchString) || s.Color.Contains(searchString)).ToList();
            }

            return View(telas);
        }

        // Mostrar el formulario para crear una nueva tela
        public IActionResult Create()
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
            if (ModelState.IsValid)
            {
                var result = await _httpClient.PostAsJsonAsync("telas/create", tela);

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(tela);
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