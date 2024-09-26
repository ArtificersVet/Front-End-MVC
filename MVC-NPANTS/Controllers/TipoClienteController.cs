using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;


namespace MVC_NPANTS.Controllers
{
    public class TipoClienteController : Controller
    {
        HttpClient _httpClient;

        public TipoClienteController(IHttpClientFactory httpClientFactory)
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
        public async Task <IActionResult> Index()
        {
            SetAuthorizationHeader();
            var tipos = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");

            if (tipos == null)
            {
                Console.WriteLine("no se encontraron los tipos de cliente");
            }

            return View(tipos);
        }

        
        public ActionResult Create()
        {
            SetAuthorizationHeader();
            return View();
        }

       
        [HttpPost]
        public async Task <IActionResult> Create(TipoCliente TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PostAsJsonAsync("tipoclientes/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        

        
        public async Task <IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo cliente");

            return View(tipo);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoCliente TiposOBJ)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.PutAsJsonAsync($"tipoclientes/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }


        public async Task<IActionResult> GetById(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        
        public async Task <IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var tipo = await _httpClient.DeleteAsync($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se elimino");

            return RedirectToAction(nameof(Index));
        }

       
    }
}
