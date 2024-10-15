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
                Console.WriteLine("no se encontraron los estilos");
            }

            return View(estilos);
        }

        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            // Obtener todas las tallas para el select
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");

            ViewBag.Tallas = tallas;  // Pasar tallas a la vista

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Estilo estiloOBJ, List<EstiloTalla> EstiloTallas)
        {
            SetAuthorizationHeader();

            // Asegúrate de que EstiloTallas no sea nulo
            if (EstiloTallas != null && EstiloTallas.Count > 0)
            {
                estiloOBJ.EstiloTallas = EstiloTallas;
            }

            var response = await _httpClient.PostAsJsonAsync("estilos/create", estiloOBJ);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(estiloOBJ);
        }




        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync($"estilos/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estilo = System.Text.Json.JsonSerializer.Deserialize<Estilo>(content, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (estilo == null)
                    {
                        Console.WriteLine($"Estilo with ID {id} not found");
                        return NotFound();
                    }

                    // Ensure EstiloTallas is not null
                    estilo.EstiloTallas ??= new List<EstiloTalla>();

                    return View(estilo);
                }
                else
                {
                    Console.WriteLine($"Error fetching estilo with ID {id}: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Error fetching estilo");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching estilo with ID {id}: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.GetFromJsonAsync<Estilo>($"estilos/{id}");

            if (estilo == null)
            {
                Console.WriteLine("No se encontró el estilo");
                return NotFound(); // Maneja el error según sea necesario
            }

            // Obtener todas las tallas para el select
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");
            ViewBag.Tallas = tallas;  // Pasar tallas a la vista

            return View(estilo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Estilo estiloOBJ)
        {
            SetAuthorizationHeader();

            if (id != estiloOBJ.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Asegúrate de que todas las tallas tengan el EstiloId correcto
                    foreach (var talla in estiloOBJ.EstiloTallas)
                    {
                        talla.EstiloId = estiloOBJ.Id;
                    }

                    var response = await _httpClient.PutAsJsonAsync($"estilos/{id}", estiloOBJ);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error al actualizar: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el estilo: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló; volvemos a cargar las tallas y mostramos el formulario nuevamente
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");
            ViewBag.Tallas = tallas;
            return View(estiloOBJ);
        }




        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var estilo = await _httpClient.DeleteAsync($"estilos/{id}");

            if (estilo.IsSuccessStatusCode) Console.WriteLine("no se encontro el id");

            return RedirectToAction(nameof(Index));
        }
    }
}
