﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_NPANTS.Models;

namespace MVC_NPANTS.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CRMAPI");
        }

        // Método para listar todos los usuarios
        public async Task<IActionResult> Index()
        {
            List<usuario> usuarios = new List<usuario>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<usuario>>("http://localhost:3000/usuarios");
                usuarios = response ?? new List<usuario>();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener la lista de usuarios: " + ex.Message);
            }

            return View(usuarios);
        }

        // Método para obtener un usuario por su ID
        public async Task<IActionResult> Details(int id)
        {
            usuario user = null;

            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"http://localhost:3000/usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para crear un nuevo usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await GetRolesAsync(); // Pasar la lista de roles a la vista como SelectListItems
            return View(new usuario());
        }

        // Método para crear un nuevo usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Create(usuario user, List<int> selectedRoleIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    user.rolid = await GetSelectedRoleAsync(selectedRoleIds); // Asignar el rol seleccionado
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/usuarios/create", user);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al crear el usuario.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
                }
            }

            ViewBag.Roles = await GetRolesAsync(); // Recargar roles en caso de error
            return View(user);
        }

        // Método para editar un usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            usuario user = null;

            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"http://localhost:3000/usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }

                ViewBag.Roles = await GetRolesAsync(); // Cargar los roles disponibles como SelectListItems
                ViewBag.SelectedRoleId = user.rolid?.Id; // Pasar el rol seleccionado
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para editar un usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, usuario user, List<int> selectedRoleIds)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                user.rolid = await GetSelectedRoleAsync(selectedRoleIds); // Actualizar rol seleccionado

                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"http://localhost:3000/usuarios/{id}", user);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al actualizar el usuario.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
                }
            }

            ViewBag.Roles = await GetRolesAsync();
            return View(user);
        }

        // Método para eliminar un usuario (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            usuario user = null;
            try
            {
                user = await _httpClient.GetFromJsonAsync<usuario>($"http://localhost:3000/usuarios/{id}");
                if (user == null)
                {
                    return NotFound();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al obtener los detalles del usuario: " + ex.Message);
                return BadRequest();
            }

            return View(user);
        }

        // Método para confirmar la eliminación de un usuario (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:3000/usuarios/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error al eliminar el usuario.");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Error al hacer la solicitud: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        // Método para obtener todos los roles y convertirlos a SelectListItems
        private async Task<List<SelectListItem>> GetRolesAsync()
        {
            var roles = new List<SelectListItem>();

            try
            {
                var allRoles = await _httpClient.GetFromJsonAsync<List<Role>>("http://localhost:3000/roles");

                // Convertir los roles a SelectListItem
                roles = allRoles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Nombre
                }).ToList();
            }
            catch (HttpRequestException)
            {
                roles = new List<SelectListItem>();
            }

            return roles;
        }

        // Método para obtener el rol seleccionado
        private async Task<Role> GetSelectedRoleAsync(List<int> selectedRoleIds)
        {
            if (selectedRoleIds == null || selectedRoleIds.Count == 0)
                return null;

            var allRoles = await _httpClient.GetFromJsonAsync<List<Role>>("http://localhost:3000/roles");
            return allRoles.FirstOrDefault(r => selectedRoleIds.Contains(r.Id)); // Asumiendo que 'Id' es la propiedad en 'Role'
        }
    }
}
