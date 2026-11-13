using System;
using System.Collections.Generic;
using System.Text;

namespace ComunaReclamos.Data.Entities
{
    public class FotoReclamo
    {
        public int Id { get; set; }
        public int ReclamoId { get; set; }
        public Reclamo Reclamo { get; set; } = null!;

        // Guarda la ruta del archivo en disco (ej. /uploads/reclamos/abc.jpg)
        public string RutaArchivo { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }
}
