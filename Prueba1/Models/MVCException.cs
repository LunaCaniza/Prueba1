using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prueba1.Models;

namespace Prueba1.Models
{
    [Table("log_exceptions")]
    public class MVCException
    {
        public int Id { get; set; }

        public string Guid { get; set; }
        public Usuarios User { get; set; }
        public int UserId { get; set; }

        public string Message { get; set; }
        public string Trace { get; set; }
        public string Path { get; set; }

        public DateTime? Handled { get; set; }

        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime? Updated_at { get; set; }
        public DateTime? Deleted_at { get; set; }

    }
}