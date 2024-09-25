public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verificar si el token de acceso existe en la sesión
        var token = context.Session.GetString("AccessToken");

        // Permitir acceso sin token a la acción "Create" del UsuarioController
        var path = context.Request.Path.ToString();
        if (string.IsNullOrEmpty(token) &&
            !path.Equals("/Login", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/Login/", StringComparison.OrdinalIgnoreCase) &&
            !path.Equals("/Usuario/Create", StringComparison.OrdinalIgnoreCase))
        {
            // Redirigir a la página de inicio de sesión
            context.Response.Redirect("/Login");
            return;
        }

        // Llamar al siguiente middleware en la cadena
        await _next(context);
    }
}
