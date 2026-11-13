using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComunaReclamos.Data.Entities
{
    public class Reclamo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoSeguimiento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Elegí un tipo de problema.")]
        public int CategoriaReclamoId { get; set; }

        public CategoriaReclamo CategoriaReclamo { get; set; } = null!;

        [Required(ErrorMessage = "Describí el problema que reportás.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 2000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Especificá la dirección del problema.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 500 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(200)]
        public string Barrio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indicá tu nombre.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres.")]
        public string NombreVecino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indicá tu teléfono de contacto.")]
        [Phone(ErrorMessage = "El teléfono no es válido. Usá el formato: 223-1234567 o 2231234567")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 20 caracteres.")]
        public string TelefonoContacto { get; set; } = string.Empty;

        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [StringLength(50)]
        public string Urgencia { get; set; } = "Media";

        [StringLength(50)]
        public string Estado { get; set; } = "Recibido";

        [StringLength(1000)]
        public string DatosContacto { get; set; } = string.Empty;

        public bool OcultarNombre { get; set; } = false;

        public int? CuadrillaId { get; set; }
        public Cuadrilla? Cuadrilla { get; set; }

        [StringLength(2000)]
        public string? NotaInterna { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<FotoReclamo> Fotos { get; set; } = new List<FotoReclamo>();
        public ICollection<HistorialReclamo> Historial { get; set; } = new List<HistorialReclamo>();
    }
}
