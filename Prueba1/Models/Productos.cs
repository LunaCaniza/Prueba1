using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Prueba1.Models
{
    public class Productos
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }

        public DateTime CreadoEm { get; set; }
        public DateTime? ActualizadoEm { get; set; }
        public DateTime? DeletadoEm { get; set; }
    }
}