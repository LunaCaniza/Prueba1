using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba1.Models
{
    public class Util
    {
        public static string DefaultGuaraniFormat(decimal valor)
        {
            return $"Gs. {valor.ToString("n0")}";
        }

        public static string DefaultGuaraniFormat(double valor)
        {
            return $"Gs. {valor.ToString("n0")}";
        }


        public static string UploadFile(IFormFile image, string path, string ImageName)
        {
            //Get url To Save
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), path, ImageName);

            using (var stream = new FileStream(SavePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }


            return ImageName;
        }
        public static string UploadFile(IFormFile image, string path)
        {
            string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            //Get url To Save
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), path, ImageName);


            using (var stream = new FileStream(SavePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }


            return ImageName;
        }
        public static bool DeleteFile(string path)
        {
            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                fi.Delete();
                return true;
            }
            else
            {
                fi.Delete();
                return false;
            }

        }

    }
}