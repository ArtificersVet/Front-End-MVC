using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class PrendaVestirController : Controller
    {
        private readonly HttpClient _httpClient;

        public PrendaVestirController(IHttpClientFactory httpClientFactory)
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

        // GET: PrendaVestirController/Index
        public async Task<IActionResult> Index(int page = 1)
        {
            SetAuthorizationHeader();

            var pagedResponse = await _httpClient.GetFromJsonAsync<PagePrendaVestirResponse>($"prendas?page={page}");

            var viewModel = new PagePrendaVestirResponse
            {
                PrendasVestir = pagedResponse?.PrendasVestir,
                CurrentPage = pagedResponse?.CurrentPage ?? 1,
                TotalPages = pagedResponse?.TotalPages ?? 1,
                PageSize = pagedResponse?.PageSize ?? 10
            };

            return View(viewModel);
        }

        // GET: PrendaVestirController/Details/5
        public async Task<IActionResult> Details(long id)
        {
            SetAuthorizationHeader();
            var prendaVestir = await _httpClient.GetFromJsonAsync<PrendaVestir>($"prendas/{id}");

            if (prendaVestir == null)
            {
                return NotFound();
            }

            return View(prendaVestir);
        }

        // GET: PrendaVestirController/Create
        public async Task<IActionResult> Create()
        {
            SetAuthorizationHeader();

            // Obtener la lista de estilos
            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");

            if (estilos == null || !estilos.Any())
            {
                ModelState.AddModelError("", "No se encontraron estilos.");
                ViewBag.Estilos = new SelectList(Enumerable.Empty<EstilosResponse>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.Estilos = new SelectList(estilos, "Id", "Nombre");
            }

            // Obtener la lista de telas
            var telasResponse = await _httpClient.GetFromJsonAsync<PageTelaResponse>("telas");
            var telas = telasResponse?.Telas;

            if (telas == null || !telas.Any())
            {
                ModelState.AddModelError("", "No se encontraron telas.");
                ViewBag.Telas = new SelectList(Enumerable.Empty<PageTelaResponse>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.Telas = new SelectList(telas, "Id", "Nombre");
            }

            // Obtener la lista de tipos de prenda
            var tiposPrendaResponse = await _httpClient.GetFromJsonAsync<PaginacionTipoPrendas>("tipoprendasvestir");
            var tiposPrenda = tiposPrendaResponse?.TipoPrendas;

            if (tiposPrenda == null || !tiposPrenda.Any())
            {
                ModelState.AddModelError("", "No se encontraron tipos de prenda.");
                ViewBag.TiposPrendaVestir = new SelectList(Enumerable.Empty<PaginacionTipoPrendas>(), "Id", "Nombre");
            }
            else
            {
                ViewBag.TiposPrendaVestir = new SelectList(tiposPrenda, "Id", "Nombre");
            }

            return View();
        }

        // POST: PrendaVestirController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrendaVestir prendaVestir)
        {
            SetAuthorizationHeader();

            if (ModelState.IsValid)
            {
                // Validar que se haya seleccionado un tipo de prenda
                if (prendaVestir.TipoprendavestirId == null)
                {
                    ModelState.AddModelError("", "Debe seleccionar un tipo de prenda.");
                }
                else
                {
                    var response = await _httpClient.PostAsJsonAsync("prendas/create", prendaVestir);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "No se pudo crear la prenda de vestir.");
                }
            }

            // Obtener nuevamente las listas en caso de que haya errores
            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");

            ViewBag.Estilos = new SelectList(estilos ?? Enumerable.Empty<Estilo>(), "Id", "Nombre");

            var telasResponse = await _httpClient.GetFromJsonAsync<Tela>("telas");
            var telas = telasResponse?.Telas;
            ViewBag.Telas = new SelectList(telas ?? Enumerable.Empty<Tela>(), "Id", "Nombre");

            var tiposPrendaResponse = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>("tipoprendasvestir");
            var tiposPrenda = tiposPrendaResponse?.TiposPrenda;
            ViewBag.TiposPrendaVestir = new SelectList(tiposPrenda ?? Enumerable.Empty<TipoPrendaVestir>(), "Id", "Nombre");

            return View(prendaVestir);
        }

        // GET: PrendaVestirController/Edit/5
        // GET: PrendaVestirController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            SetAuthorizationHeader();

            // Obtener la prenda a editar
            var prendaVestir = await _httpClient.GetFromJsonAsync<PrendaVestir>($"prendas/{id}");

            if (prendaVestir == null)
            {
                return NotFound();
            }

            // Obtener la lista de estilos
            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");


            // Obtener la lista de telas
            var telasResponse = await _httpClient.GetFromJsonAsync<PageTelaResponse>("telas");
            var telas = telasResponse?.Telas ?? new List<Tela>();

            // Obtener la lista de tipos de prenda
            var tiposPrendaResponse = await _httpClient.GetFromJsonAsync<PaginacionTipoPrendas>("tipoprendasvestir");
            var tiposPrendaVestir = tiposPrendaResponse?.TipoPrendas ?? new List<TipoPrendaVestir>();

            // Establecer los ViewBag
            ViewBag.Estilos = new SelectList(estilos, "Id", "Nombre", prendaVestir.EstiloId);
            ViewBag.Telas = new SelectList(telas, "Id", "Nombre", prendaVestir.TelaId);
            ViewBag.TiposPrendaVestir = new SelectList(tiposPrendaVestir, "Id", "Nombre", prendaVestir.TipoprendavestirId);

            return View(prendaVestir);
        }

        // POST: PrendaVestirController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PrendaVestir prendaVestir)
        {
            SetAuthorizationHeader();

            if (ModelState.IsValid)
            {
                // Realizar la actualización en la API
                var response = await _httpClient.PutAsJsonAsync($"prendas/{id}", prendaVestir);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo actualizar la prenda de vestir.");
            }

            // Obtener nuevamente las listas en caso de que haya errores
            var estilosResponse = await _httpClient.GetFromJsonAsync<EstilosResponse>("estilos");
            var estilos = estilosResponse?.Estilos ?? new List<Estilo>();

            var telasResponse = await _httpClient.GetFromJsonAsync<PageTelaResponse>("telas");
            var telas = telasResponse?.Telas ?? new List<Tela>();

            var tiposPrendaResponse = await _httpClient.GetFromJsonAsync<PaginacionTipoPrendas>("tipoprendasvestir");
            var tiposPrendaVestir = tiposPrendaResponse?.TipoPrendas ?? new List<TipoPrendaVestir>();

            // Establecer los ViewBag
            ViewBag.Estilos = new SelectList(estilos, "Id", "Nombre", prendaVestir.EstiloId);
            ViewBag.Telas = new SelectList(telas, "Id", "Nombre", prendaVestir.TelaId);
            ViewBag.TiposPrendaVestir = new SelectList(tiposPrendaVestir, "Id", "Nombre", prendaVestir.TipoprendavestirId);

            return View(prendaVestir);
        }


        // GET: PrendaVestirController/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            SetAuthorizationHeader();
            var prendaVestir = await _httpClient.GetFromJsonAsync<PrendaVestir>($"prendas/{id}");

            if (prendaVestir == null)
            {
                return NotFound();
            }

            return View(prendaVestir);
        }

        // POST: PrendaVestirController/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"prendas/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo eliminar la prenda .");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Error al eliminar prenda.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
