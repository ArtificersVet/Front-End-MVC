using MVC_NPANTS.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Habilitar el uso de sesiones

builder.Services.AddHttpClient("CRMAPI", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["UrlsAPI:CRM"]); // Aseg�rate de que esta configuraci�n est� en appsettings.json
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Habilitar sesiones antes de la autorizaci�n

// Registrar el middleware de autenticaci�n
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
