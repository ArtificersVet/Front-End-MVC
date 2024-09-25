namespace MVC_NPANTS.Models
{
    public class usuario
    {
        public int id { get; set; }
        public List<roles> rolid { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
