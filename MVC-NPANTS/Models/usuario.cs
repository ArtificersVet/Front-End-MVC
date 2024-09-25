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
}
