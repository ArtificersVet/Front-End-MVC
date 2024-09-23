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
        public async Task<List<roles>> Index()
        {
            try
            {
                // URL completa de la API
                var response = await _httpClient.GetFromJsonAsync<List<roles>>("http://localhost:3000/roles");

                if (response == null)
                {
                    Console.WriteLine("No se encontraron roles.");
                    return new List<roles>(); 
                }

                return response; 
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                return new List<roles>(); 
            }
        }
    }
}
