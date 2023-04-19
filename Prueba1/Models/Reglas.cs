using System;
using System.ComponentModel;

namespace Prueba1.Models
{
    public class Reglas
    {
        public int Id { get; set; }

        public string Regla { get; set; }

        public Reglas Max { get; set; }
        [DisplayName("Regla Max.")]
        public int? MaxId { get; set; }

        public DateTime CreadoEm { get; set; }
        public DateTime? ActualizadoEm { get; set; }
        public DateTime? DeletadoEm { get; set; }

    }
}