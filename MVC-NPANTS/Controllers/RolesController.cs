using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _httpClient;

        public RolesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }
        public async Task<IActionResult> Index()
        {
            List<roles> roles = new List<roles>();

            try
            {
                // URL completa de la API
                var response = await _httpClient.GetFromJsonAsync<List<roles>>("http://localhost:3000/roles");

                if (response != null)
                {
                    roles = response;
                }
                else
                {
                    Console.WriteLine("No se encontraron roles.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }

            // Retornamos la vista con el modelo
            return View(roles);
        }
    }
}
