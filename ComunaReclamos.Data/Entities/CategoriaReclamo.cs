using System;
using System.Collections.Generic;
using System.Text;

namespace ComunaReclamos.Data.Entities
{
    public class CategoriaReclamo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<Reclamo> Reclamos { get; set; } = new List<Reclamo>();

    }
}
