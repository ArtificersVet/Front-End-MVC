using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public class usuario
    {
        public int id { get; set; }
        public virtual Role? rolid { get; set; } // Use 'Role' instead of 'roles'
        public string nombre { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    public class usuariosPaged
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        [JsonPropertyName("usuarios")]
        public List<usuario> usuarios { get; set; } = new List<usuario>();
    }
}
