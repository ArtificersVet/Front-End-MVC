using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MVC_NPANTS.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }

        // Método para mostrar la vista de inicio de sesión
        public IActionResult Index()
        {
            return View();
        }

        // Método para manejar el inicio de sesión
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginData = new
            {
                email,
                password
            };

            // Convertir los datos de inicio de sesión a JSON
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Enviar la solicitud POST a la API
            var response = await _httpClient.PostAsync("/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<LoginResponse>();
                // Almacenar el token en una cookie o en la sesión
                HttpContext.Session.SetString("AccessToken", responseData.AccessToken);

                return RedirectToAction("Index", "Home"); // Redirigir a la página de inicio
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Inicio de sesión fallido. Verifique sus credenciales. Gran pendejo.");
                return View("Index"); // Vuelve a la vista de inicio de sesión
            }
        }

        // Clase para deserializar la respuesta de la API
        public class LoginResponse
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string AccessToken { get; set; }
        }
    }
}
