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
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();

            var prendasVestir = await _httpClient.GetFromJsonAsync<List<PrendaVestir>>("prendas");

            return View(prendasVestir ?? new List<PrendaVestir>());
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

            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");
            var telas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");
            var tiposPrendaVestir = await _httpClient.GetFromJsonAsync<List<TipoPrendaVestir>>("tipoprendasvestir");

            ViewBag.Estilos = new SelectList(estilos, "Id", "Nombre");
            ViewBag.Telas = new SelectList(telas, "Id", "Nombre");
            ViewBag.TiposPrendaVestir = new SelectList(tiposPrendaVestir, "Id", "Nombre");

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
                var response = await _httpClient.PostAsJsonAsync("prendas/create", prendaVestir);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo crear la prenda de vestir.");
            }

            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");
            var telas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");
            var tiposPrendaVestir = await _httpClient.GetFromJsonAsync<List<TipoPrendaVestir>>("tipoprendasvestir");

            ViewBag.Estilos = new SelectList(estilos, "Id", "Nombre");
            ViewBag.Telas = new SelectList(telas, "Id", "Nombre");
            ViewBag.TiposPrendaVestir = new SelectList(tiposPrendaVestir, "Id", "Nombre");

            return View(prendaVestir);
        }

        // GET: PrendaVestirController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            SetAuthorizationHeader();
            var prendaVestir = await _httpClient.GetFromJsonAsync<PrendaVestir>($"prendas/{id}");

            if (prendaVestir == null)
            {
                return NotFound();
            }

            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");
            var telas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");
            var tiposPrendaVestir = await _httpClient.GetFromJsonAsync<List<TipoPrendaVestir>>("tipoprendasvestir");

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
                var response = await _httpClient.PutAsJsonAsync($"prendas/{prendaVestir.Id}", prendaVestir);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "No se pudo actualizar la prenda de vestir.");
            }

            var estilos = await _httpClient.GetFromJsonAsync<List<Estilo>>("estilos");
            var telas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");
            var tiposPrendaVestir = await _httpClient.GetFromJsonAsync<List<TipoPrendaVestir>>("tipoprendasvestir");

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
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"prendas/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "No se pudo eliminar la prenda de vestir.");
            return RedirectToAction(nameof(Index));
        }
    }
}
