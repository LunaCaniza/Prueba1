using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Prueba1.Models
{
    public class Usuarios
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Nombre")]
        [MinLength(10)]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Correo Electrónico")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Fueto")]
        public string Fueto { get; set; }

        [Required]
        [DisplayName("Contraseña")]
        [MinLength(6)]
        public string Contrasena { get; set; }

        public DateTime CreadoEm { get; set; }
        public DateTime? ActualizadoEm { get; set; }
        public DateTime? DeletadoEm { get; set; }

        public ICollection<Permisos> Permisos { get; set; }

        //Salvar a senha em hash
        public static string GetSha1(string str)
        {
            var data = Encoding.ASCII.GetBytes(str);
            var hashData = new SHA1Managed().ComputeHash(data);
            string hash = "";

            foreach (var ch in hashData)
            {
                hash += ch.ToString("X2");
            }

            return hash;
        }

        public string FuetoPath()
        {
            if (String.IsNullOrEmpty(Fueto))
                return "/Uploads/Avatars/avatar.png";
            else
                return Fueto;
        }
    }
}