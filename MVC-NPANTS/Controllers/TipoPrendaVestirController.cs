using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;


namespace MVC_NPANTS.Controllers
{
    public class TipoPrendaVestirController : Controller
    {
        HttpClient _httpClient;

        public TipoPrendaVestirController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> Index(int page = 1)
        {
            SetAuthorizationHeader();
            var pagedResponse = await _httpClient.GetFromJsonAsync<PaginacionTipoPrendas>($"tipoprendasvestir?page={page}");

            var viewModel = new PaginacionTipoPrendas
            {
                TipoPrendas = pagedResponse?.TipoPrendas,
                CurrentPage = pagedResponse?.CurrentPage ?? 1,
                TotalPages = pagedResponse?.TotalPages ?? 1,
                PageSize = pagedResponse?.PageSize ?? 10
            };
            return View(viewModel);
        }

       
       

        // GET: TipoPrendaVestirController/Create
        public ActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

        // POST: TipoPrendaVestirController/Create
        [HttpPost]

        public async Task<IActionResult>  Create(TipoPrendaVestir TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PostAsJsonAsync("tipoprendasvestir/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo prenda");

            return View(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoPrendaVestir TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PutAsJsonAsync($"tipoprendasvestir/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.DeleteAsync($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se eliminó");

            return RedirectToAction(nameof(Index));
        }
    }
}
