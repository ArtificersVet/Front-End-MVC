using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MVC_NPANTS.Controllers
{
    public class EstiloController : Controller
    {
        private readonly HttpClient _httpClient;

        public EstiloController(IHttpClientFactory httpClientFactory)
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

        // GET: Estilo
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");
            return View(estilos);
        }

        // GET: Estilo/Details/5
        public async Task<IActionResult> Details(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");

            if (estilo == null)
            {
                return NotFound();
            }

            return View(estilo);
        }

        // GET: Estilo/Create
        // GET: Estilo/Create
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            // Obtener tallas de la API
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("talla");

            if (tallas == null || !tallas.Any())
            {
                ViewBag.ErrorMessage = "No se pudieron cargar las tallas.";
                tallas = new List<Talla>(); // Evita posibles errores de null
            }

            // Asignar la lista de tallas al ViewBag
            ViewBag.Tallas = tallas;

            // Crear el modelo y convertir las tallas si es necesario
            var viewModel = new EstiloCreateViewModel
            {
                Tallas = new List<EstiloTallaCreateDTO>() // Cambia a EstiloTallaCreateDTO si es necesario
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre, Tipo, Tallas")] EstiloCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                SetAuthorizationHeader();

                try
                {
                    // Crear el objeto que se enviará a la API con el formato correcto
                    var estiloRequest = new
                    {
                        nombre = viewModel.Nombre,
                        tipo = viewModel.Tipo,
                        tallas = viewModel.Tallas.Select(t => new
                        {
                            talla_id  = t.TallaId,
                            consumoTela = t.ConsumoTela
                        }).ToList()
                    };

                    // Log para verificar el JSON de solicitud
                    Console.WriteLine("Request data: " + JsonSerializer.Serialize(estiloRequest));

                    // Hacer la solicitud a la API
                    var response = await _httpClient.PostAsJsonAsync("estilos/create", estiloRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    // Leer el mensaje de error de la respuesta
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al crear el estilo: {errorContent}");

                    // Log del error para más detalles
                    Console.WriteLine($"Response error: {errorContent}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear el estilo: {ex.Message}");
                }
            }

            // Si falla, recarga las tallas para mostrarlas en la vista
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("talla");
            ViewBag.Tallas = tallas;
            return View(viewModel);
        }

        // GET: Estilo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            try
            {
                // Obtener el estilo con sus tallas
                var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");
                if (estilo == null)
                {
                    return NotFound();
                }

                // Obtener todas las tallas disponibles
                var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("talla");
                if (tallas == null)
                {
                    tallas = new List<Talla>();
                }

                // Crear el ViewModel
                var viewModel = new EstiloEditViewModel
                {
                    Id = (int)estilo.Id,
                    Nombre = estilo.Nombre,
                    Tipo = estilo.Tipo,
                    Tallas = estilo.Tallas.Select(t => new EstiloTallaEditDTO
                    {
                        Id = t.Id,
                        TallaId = (int)t.Talla.Id,
                        ConsumoTela = t.ConsumoTela
                    }).ToList()
                };

                // Guardar las tallas en ViewBag para el dropdown
                ViewBag.Tallas = tallas;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el estilo: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] EstiloEditViewModel viewModel, [FromForm] List<int> TallasEliminadas)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                SetAuthorizationHeader();
                try
                {
                    // Crear el objeto para la API
                    var estiloUpdate = new
                    {
                        id = viewModel.Id,
                        nombre = viewModel.Nombre,
                        tipo = viewModel.Tipo,
                        tallas = viewModel.Tallas.ToList(), // Usar la lista completa de Tallas sin modificar
                        tallasEliminadas = TallasEliminadas ?? new List<int>()
                    };

                    // Realizar la petición PUT
                    var response = await _httpClient.PutAsJsonAsync($"estilos/{id}", estiloUpdate);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Estilo actualizado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error al actualizar: {errorContent}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al actualizar el estilo: {ex.Message}");
                }
            }

            // Si llegamos aquí, algo falló. Recargar las tallas y volver a mostrar el formulario
            return await LoadTallasAndReturnView(viewModel);
        }

        private async Task<IActionResult> LoadTallasAndReturnView(EstiloEditViewModel viewModel)
        {
            // Obtener la lista de tallas y asignarla al ViewBag
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("talla");
            ViewBag.Tallas = tallas;
            return View(viewModel);
        }


        // GET: Estilo/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");

            if (estilo == null)
            {
                return NotFound();
            }

            return View(estilo);
        }

        // POST: Estilo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"estilos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Hubo un error al eliminar el estilo. Código de estado: {response.StatusCode}. Detalles: {errorContent}";
                    // Puedes registrar el error o mostrar un mensaje al usuario
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Hubo un error al eliminar el estilo: {ex.Message}";
                // Puedes registrar el error o mostrar un mensaje al usuario
            }

            // Si hay un error, redirigir a la página Index
            return RedirectToAction(nameof(Index));
        }
    }
}