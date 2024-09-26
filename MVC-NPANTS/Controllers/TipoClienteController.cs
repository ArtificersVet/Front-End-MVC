using Microsoft.AspNetCore.Mvc;
using MVC_NPANTS.Models;
using NuGet.Protocol;

namespace MVC_NPANTS.Controllers
{
    public class TipoClienteController : Controller
    {
        HttpClient _httpClient;

        public TipoClienteController(IHttpClientFactory httpClientFactory)
        {

            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }
        public async Task <IActionResult> Index()
        {
            var tipos = await _httpClient.GetFromJsonAsync<List<TipoCliente>>("tipoclientes");

            if (tipos == null)
            {
                Console.WriteLine("no se encontraron los tipos de cliente");
            }

            return View(tipos);
        }

        
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        public async Task <ActionResult> Create(TipoCliente TiposOBJ)
        {
            var tipo = await _httpClient.PostAsJsonAsync("tipoclientes/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        

        
        public async Task <IActionResult> Edit(int id)
        {
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo cliente");

            return View(tipo);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoCliente TiposOBJ)
        {
            var tipo = await _httpClient.PutAsJsonAsync($"tipoclientes/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }


        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _httpClient.GetFromJsonAsync<TipoCliente>($"tipoclientes/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        
        public async Task <IActionResult> Delete(int id)
        {
            var tipo = await _httpClient.DeleteAsync($"tipoclientes/{id}");

            if (tipo == null) Console.WriteLine("No se elimino");

            return RedirectToAction(nameof(Index));
        }

       
    }
}
