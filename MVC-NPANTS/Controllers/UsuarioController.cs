using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;
using System.Net.Http.Json;

namespace MVC_NPANTS.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(IHttpClientFactory httpClientFactory)
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

        // Método para listar todos los usuarios
        // Método para listar todos los usuarios con paginación
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchString = null)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            List<usuario> usuarios = new List<usuario>();
            int totalPages = 0;

            try
            {
                // Realizar solicitud con paginación
                var response = await _httpClient.GetFromJsonAsync<usuariosPaged>("usuarios?page=" + page + "&pageSize=" + pageSize);
                if (response != null)
                {
                    usuarios = response.usuarios;
                    totalPages = response.TotalPages;
                }

                // Filtrar usuarios si hay un término de búsqueda
                if (!string.IsNullOrEmpty(searchString))
                {
                    usuarios = usuarios.Where(u => u.nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                                    u.email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    ViewBag.SearchString = searchString; // Guardar el término de búsqueda en ViewBag
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener la lista de usuarios: " + ex.Message);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(usuarios);
        }
    



        // Método para obtener un usuario por su ID
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            usuario user = null;
            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para crear un nuevo usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            ViewBag.Roles = await GetRolesAsync(); // Pasar la lista de roles a la vista como SelectListItems
            return View(new usuario());
        }

        // Método para crear un nuevo usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Create(usuario user, List<int> selectedRoleIds)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            if (ModelState.IsValid)
            {
                try
                {
                    user.rolid = await GetSelectedRoleAsync(selectedRoleIds); // Asignar el rol seleccionado
                    var response = await _httpClient.PostAsJsonAsync("usuarios/create", user);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al crear el usuario.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
                }
            }

            ViewBag.Roles = await GetRolesAsync(); // Recargar roles en caso de error
            return View(user);
        }

        // Método para editar un usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            usuario user = null;
            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }

                ViewBag.Roles = await GetRolesAsync(); // Cargar los roles disponibles como SelectListItems
                ViewBag.SelectedRoleId = user.rolid?.Id; // Pasar el rol seleccionado
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para editar un usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, usuario user, List<int> selectedRoleIds)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            if (id != user.id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                user.rolid = await GetSelectedRoleAsync(selectedRoleIds); // Actualizar rol seleccionado

                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"usuarios/{id}", user);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al actualizar el usuario.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
                }
            }

            ViewBag.Roles = await GetRolesAsync();
            return View(user);
        }

        // Método para eliminar un usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            usuario user = null;
            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para confirmar la eliminación de un usuario (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetAuthorizationHeader(); // Establecer el encabezado de autorización

            try
            {
                var response = await _httpClient.DeleteAsync($"usuarios/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error al eliminar el usuario.");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        // Método para obtener todos los roles y convertirlos a SelectListItems
        private async Task<List<SelectListItem>> GetRolesAsync()
        {
            var roles = new List<SelectListItem>();

            try
            {
                var allRoles = await _httpClient.GetFromJsonAsync<List<Role>>("roles");

                // Convertir los roles a SelectListItem
                roles = allRoles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Nombre
                }).ToList();
            }
            catch (HttpRequestException)
            {
                roles = new List<SelectListItem>();
            }

            return roles;
        }

        // Método para obtener el rol seleccionado
        private async Task<Role> GetSelectedRoleAsync(List<int> selectedRoleIds)
        {
            if (selectedRoleIds == null || selectedRoleIds.Count == 0)
                return null;

            var allRoles = await _httpClient.GetFromJsonAsync<List<Role>>("roles");
            return allRoles.FirstOrDefault(r => selectedRoleIds.Contains(r.Id)); // Asumiendo que 'Id' es la propiedad en 'Role'
        }
    }
    // Clase para manejar la respuesta con paginación
    public class PaginationResponse
    {
        public int TotalPages { get; set; }
        public List<usuario> Usuarios { get; set; }
    }
}
