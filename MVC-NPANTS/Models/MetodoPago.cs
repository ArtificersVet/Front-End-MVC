using System;
using System.Collections.Generic;

namespace MVC_NPANTS.Models
{
    public partial class MetodoPago
    {
        public MetodoPago()
        {
            Pagos = new HashSet<Pago>();
        }

        public long Id { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
