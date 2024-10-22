using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MVC_NPANTS.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _httpClient;

        public RolesController(IHttpClientFactory httpClientFactory)
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
        // Método para listar todos los roles
         public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            List<Role> roles = new List<Role>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Role>>("/roles");
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

            return View(roles);
        }


        // Método para obtener un rol por su ID
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            Role role = null;

            try
            {
                role = await _httpClient.GetFromJsonAsync<Role>($"https://api-npants.onrender.com/roles/{id}");
                if (role == null)
                {
                    Console.WriteLine($"No se encontró el rol con ID {id}.");
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                return BadRequest();
            }

            return View(role);
        }

        // Método para crear un nuevo rol (GET)
        [HttpGet]
        public IActionResult Create()
        {
            SetAuthorizationHeader();
            var role = new Role(); // Initialize a new instance of Role
            return View(role); // Pass it to the view
        }

        // Método para crear un nuevo rol (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            SetAuthorizationHeader();
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("https://api-npants.onrender.com/roles/create", role);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine($"Error al crear el rol: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                }
            }
            return View(role);
        }

        // Método para editar un rol (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            Role role = null;
            try
            {
                role = await _httpClient.GetFromJsonAsync<Role>($"https://api-npants.onrender.com/roles/{id}");
                if (role == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                return BadRequest();
            }

            return View(role);
        }

        // Método para editar un rol (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Role role)
        {

            SetAuthorizationHeader();
            if (id != role.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"https://api-npants.onrender.com/roles/{id}", role);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine($"Error al actualizar el rol: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                }
            }

            return View(role);
        }

        // Método para confirmar la eliminación de un rol (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            Role role = null;
            try
            {
                role = await _httpClient.GetFromJsonAsync<Role>($"https://api-npants.onrender.com/roles/{id}");
                if (role == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                return BadRequest();
            }

            return View(role);
        }

        // Método para manejar la confirmación de eliminación (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"https://api-npants.onrender.com/roles/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index"); // Redirigir a la lista de roles después de eliminar
                }
                else
                {
                    Console.WriteLine($"Error al eliminar el rol: {response.StatusCode}");
                    return BadRequest();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                return BadRequest();
            }
        }
    }
}
