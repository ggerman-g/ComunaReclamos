using System;
using System.Collections.Generic;
using System.Text;

namespace ComunaReclamos.Data.Entities
{
    public class HistorialReclamo
    {
        public int Id { get; set; }
        public int ReclamoId { get; set; }
        public Reclamo Reclamo { get; set; } = null!;

        public string EstadoAnterior { get; set; } = string.Empty;
        public string EstadoNuevo { get; set; } = string.Empty;

        // Comentario visible para el vecino en la línea de tiempo
        public string ComentarioVecino { get; set; } = string.Empty;

        public DateTime FechaCambio { get; set; } = DateTime.Now;
    }
}
