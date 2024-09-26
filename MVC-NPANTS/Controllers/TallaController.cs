﻿using Microsoft.AspNetCore.Mvc;
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

        // Listar todas las tallas
        public async Task<IActionResult> Index()
        {
            var tallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");

            if (tallas == null)
            {
                Console.WriteLine("No se encontraron tallas");
            }

            return View(tallas);
        }

        // Mostrar el formulario para crear una nueva talla
        public IActionResult Create()
        {
            return View();
        }

        // Procesar el formulario de creación de talla
        [HttpPost]
        public async Task<IActionResult> Create(Talla talla)
        {
            var result = await _httpClient.PostAsJsonAsync("tallas/create", talla);

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(talla);
        }

        // Obtener los detalles de una talla por ID
        public async Task<IActionResult> Details(int id)
        {
            var talla = await _httpClient.GetFromJsonAsync<Talla>($"tallas/{id}");

            if (talla == null)
            {
                Console.WriteLine($"No se encontró la talla con ID {id}");
            }

            return View(talla);
        }

        // Mostrar el formulario de edición de una talla
        public async Task<IActionResult> Edit(int id)
        {
            var talla = await _httpClient.GetFromJsonAsync<Talla>($"tallas/{id}");

            if (talla == null)
            {
                Console.WriteLine("No se encontró la talla");
            }

            return View(talla);
        }

        // Procesar el formulario de edición
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Talla talla)
        {
            var result = await _httpClient.PutAsJsonAsync($"tallas/{id}", talla);

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(talla);
        }

        // Eliminar una talla
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"tallas/{id}");

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("No se pudo eliminar la talla");
            return RedirectToAction(nameof(Index));
        }
    }
}