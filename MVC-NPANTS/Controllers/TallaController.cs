using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Net.Http.Json;

namespace MVC_NPANTS.Controllers
{
    public class TallaController : Controller
    {
        private readonly HttpClient _httpClient;

        public TallaController(IHttpClientFactory httpClientFactory)
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

        // Modelo para la respuesta paginada
        public class PaginatedResponse<T>
        {
            public List<T> Data { get; set; }
            public PaginationInfo Pagination { get; set; }
        }

        public class PaginationInfo
        {
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int ItemsPerPage { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPrevPage { get; set; }
        }

        // Listar todas las tallas con paginación
        public async Task<IActionResult> Index(int page = 1, int limit = 10)
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<Talla>>($"tallas?page={page}&limit={limit}");

                if (response == null || response.Data == null)
                {
                    TempData["Error"] = "No se encontraron tallas";
                    return View(new PaginatedResponse<Talla>
                    {
                        Data = new List<Talla>(),
                        Pagination = new PaginationInfo()
                    });
                }

                // Pasar los datos de paginación a la vista a través de ViewBag
                ViewBag.CurrentPage = response.Pagination.CurrentPage;
                ViewBag.TotalPages = response.Pagination.TotalPages;
                ViewBag.HasNextPage = response.Pagination.HasNextPage;
                ViewBag.HasPrevPage = response.Pagination.HasPrevPage;
                ViewBag.ItemsPerPage = response.Pagination.ItemsPerPage;
                ViewBag.TotalItems = response.Pagination.TotalItems;

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las tallas: " + ex.Message;
                return View(new PaginatedResponse<Talla>
                {
                    Data = new List<Talla>(),
                    Pagination = new PaginationInfo()
                });
            }
        }

        // El resto de los métodos permanecen igual
        public IActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Talla talla)
        {
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View(talla);
            }

            var result = await _httpClient.PostAsJsonAsync("tallas/create", talla);

            if (result.IsSuccessStatusCode)
            {
                TempData["Success"] = "Talla creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "No se pudo crear la talla");
            return View(talla);
        }

        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var talla = await _httpClient.GetFromJsonAsync<Talla>($"tallas/{id}");

                if (talla == null)
                {
                    TempData["Error"] = $"No se encontró la talla con ID {id}";
                    return RedirectToAction(nameof(Index));
                }

                return View(talla);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var talla = await _httpClient.GetFromJsonAsync<Talla>($"tallas/{id}");

                if (talla == null)
                {
                    TempData["Error"] = "No se encontró la talla";
                    return RedirectToAction(nameof(Index));
                }

                return View(talla);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la talla para editar: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Talla talla)
        {
            SetAuthorizationHeader();
            try
            {
                var result = await _httpClient.PutAsJsonAsync($"tallas/{id}", talla);

                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talla actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No se pudo actualizar la talla";
                return View(talla);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la talla: " + ex.Message;
                return View(talla);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var result = await _httpClient.DeleteAsync($"tallas/{id}");

                if (result.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Talla eliminada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No se pudo eliminar la talla";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la talla: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}