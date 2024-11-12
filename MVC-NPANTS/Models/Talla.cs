using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MVC_NPANTS.Models
{
    public  class Talla
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }

    }

    public class TallasResponse
    {
        //public int TotalItems { get; set; }
        //public int TotalPages { get; set; }
        //public int CurrentPage { get; set; }
        //public int PageSize { get; set; }
        //[JsonPropertyName("tallas")]
        public List<Talla> Data{ get; set; } 
    }  
}
