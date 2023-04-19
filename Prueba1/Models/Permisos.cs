using System;
using Prueba1.Models;

namespace Prueba1.Models
{
    public class Permisos
    {
        public int Id { get; set; }

        public Usuarios Usuario { get; set; }
        public int UsuarioId { get; set; }

        public Reglas Regla { get; set; }
        public int ReglaId { get; set; }

        public DateTime CreadoEm { get; set; }
        public DateTime? ActualizadoEm { get; set; }
        public DateTime? DeletadoEm { get; set; }
    }
}