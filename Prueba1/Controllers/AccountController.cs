using Prueba1.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Prueba1.Models;
using Microsoft.AspNetCore.Http;
using Prueba1.Models;

namespace Prueba1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        private readonly Prueba1Context _context;

        public AccountController(Prueba1Context context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string Nombre, string Password, string RedirectUrl)
        {
            /*Implementar Segurança*/
            var user = await _context.Usuarios
                                     .Include(i => i.Permisos)
                                        .ThenInclude(i => i.Regla)
                                     .FirstOrDefaultAsync(w => w.Nombre == Nombre &&
                                                               w.Contrasena == Models.Usuarios.GetSha1(Password));

            var senha = Models.Usuarios.GetSha1(Password);

            if (user != null)
            {

                var claims = new List<Claim>();

                claims.Add(new Claim("Id", user.Id.ToString()));
                claims.Add(new Claim("Nombre", user.Nombre.ToString()));
                claims.Add(new Claim("Email", user.Email));

                foreach (var permiso in user.Permisos)
                {
                    claims.Add(new Claim(ClaimTypes.Role, permiso.Regla.Regla));
                }

                var identidade = new ClaimsIdentity(claims, "Login");
                var principal = new ClaimsPrincipal(identidade);

                var propriedasCookies = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.ToLocalTime().AddDays(1),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, propriedasCookies);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        // GET: Perfil
        [Authorize]
        public async Task<IActionResult> Perfil(int? verPermissoes)
        {
            int id = Convert.ToInt32(this.User.FindFirst("Id").Value);

            var user = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);

            return View(user);
        }


        //POST : Perfil
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Perfil(int id, string senhaPassada, string novaSenha, string confirmarSenha)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);

            if (novaSenha != confirmarSenha)
            {
                ViewBag.Erro = "A nova senha e a confirmação devem ser iguais.";
                return View(user);
            }
            else
            {
                if (Usuarios.GetSha1(senhaPassada) != user.Contrasena)
                {
                    ViewBag.Erro = "A senha atual está errada.";
                    return View(user);
                }
                else
                {
                    user.Contrasena = Usuarios.GetSha1(novaSenha);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        //POST: UploadImage

        [HttpPost]
        public async Task<IActionResult> UploadFile(int uploadId, int uploadRoute, string uploadRedirect, IFormFile file)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == uploadId);

            if (user == null)
            {
                return NotFound();
            }

            string fileName = Util.UploadFile(file, "wwwroot/Uploads/Avatars/"); // documentos/ está na string do BD

            user.Fueto = $"/Uploads/Avatars/{fileName}";
            user.ActualizadoEm = DateTime.Now;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(uploadRedirect, new { id = uploadId });
        }
    }
}