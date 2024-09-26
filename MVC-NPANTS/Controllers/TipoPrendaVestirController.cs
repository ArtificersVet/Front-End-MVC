﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var tipos = await _httpClient.GetFromJsonAsync<List<TipoPrendaVestir>>("tipoprendasvestir");

            if (tipos == null)
            {
                Console.WriteLine("no se encontraron los tipos de prendas");
            }

            return View(tipos);
        }

       
       

        // GET: TipoPrendaVestirController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoPrendaVestirController/Create
        [HttpPost]

        public async Task<IActionResult>  Create(TipoPrendaVestir TiposOBJ)
        {
            var tipo = await _httpClient.PostAsJsonAsync("tipoprendasvestir/create", TiposOBJ);

            if (tipo.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se encontro el tipo prenda");

            return View(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TipoPrendaVestir TiposOBJ)
        {
            var tipo = await _httpClient.PutAsJsonAsync($"tipoprendasvestir/{id}", TiposOBJ);

            if (tipo.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var tipo = await _httpClient.GetFromJsonAsync<TipoPrendaVestir>($"tipoprendasvestir/{id}");

            if (tipo == null)
            {
                Console.WriteLine($"Found {id}");
            }

            return View(tipo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _httpClient.DeleteAsync($"tipoprendasvestir/{id}");

            if (tipo == null) Console.WriteLine("No se eliminó");

            return RedirectToAction(nameof(Index));
        }
    }
}