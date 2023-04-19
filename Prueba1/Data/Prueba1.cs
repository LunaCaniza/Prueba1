using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prueba1.Models;
using System.Reflection.Emit;
using System.Collections;
using System.Reflection.Metadata;

namespace Prueba1.Data
{
    public class Prueba1Context : DbContext
    {
        public Prueba1Context(DbContextOptions<Prueba1Context> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Permisos> Permisos { get; set; }
        public DbSet<Reglas> Reglas { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<MVCException> Exceptions { get; set; }

        internal static Task<string> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }
}