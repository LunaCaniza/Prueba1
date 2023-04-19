using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prueba1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Prueba1.Data;
using Prueba1.Models;

namespace Prueba1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Prueba1Context _context;

        public HomeController(Prueba1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var exp = new MVCException();

            try
            {
                var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
                var path = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

                var userClaim = this.User.FindFirst("Id");
                dynamic user;

                if (userClaim != null)
                    exp.UserId = Convert.ToInt32(userClaim.Value);

                exp.Message = context.Error.Message;
                exp.Trace = context.Error.StackTrace;
                exp.Path = path.Path;
                exp.Guid = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                _context.Add(exp);
                await _context.SaveChangesAsync();

            }
            catch
            {
            }

            ViewBag.Guid = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View();
        }
    }
}