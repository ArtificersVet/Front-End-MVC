using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using NuGet.Protocol;

namespace MVC_NPANTS.Controllers
{
    public class EstiloController : Controller
    {
        HttpClient _httpClient;

        public EstiloController(IHttpClientFactory httpClientFactory) {

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

        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");

            if (estilos == null)
            {
                Console.WriteLine("no se esncontraron los estilos");
            }

            return View(estilos);
        }

        public ActionResult create()
        {
            SetAuthorizationHeader();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> create(Estilo estiloOBJ)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.PostAsJsonAsync("estilos/create", estiloOBJ);

            if (estilo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");

            if (estilo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(estilo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");

            if (estilo == null) Console.WriteLine("no se encontro el estilo");

            return View(estilo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Estilo estiloOBJ)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.PutAsJsonAsync($"estilos/{id}", estiloOBJ);

            if(estilo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.DeleteAsync($"estilos/{id}");

            if (estilo == null) Console.WriteLine("no se elimino");

            return RedirectToAction(nameof(Index));
        }
    }
}
