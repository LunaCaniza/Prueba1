using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prueba1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Prueba1.Controllers
{
    [Authorize]
    public class MVCExceptionsController : Controller
    {
        private readonly Prueba1Context _context;

        public MVCExceptionsController(Prueba1Context context)
        {
            _context = context;
        }

        // GET: MVCExceptions
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Index")]
        public async Task<IActionResult> IndexData(int? draw, int? start, int? length)
        {
            int recordsFiltered;

            string search = Request.Form["search[value]"];

            var orderColumn = Convert.ToInt32(Request.Form["order[0][column]"]);
            string orderDir = Request.Form["order[0][dir]"];
            string orderColumnName = Request.Form[$"columns[{orderColumn}][name]"];

            string status = Request.Form["columns[3][search][value]"];

            draw ??= 1;
            start ??= 0;
            length ??= 25;

            var table = _context.Exceptions
                                    .Include(i => i.User)
                                    .Where(w => !w.Deleted_at.HasValue);

            int recordsTotal = await table.CountAsync();

            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                table = table.Where(w => EF.Functions.ILike(EF.Functions.Unaccent(w.Guid), "%" + search + "%"));
            }

            recordsFiltered = table.Count();

            if (orderDir == "asc")
                table = table.OrderBy(o => EF.Property<object>(o, orderColumnName));
            else
                table = table.OrderByDescending(o => EF.Property<object>(o, orderColumnName));

            table = table
                    .Skip(start.Value)
                    .Take(length.Value);

            var data = await table.ToListAsync();

            return Json(new { draw, recordsTotal, recordsFiltered, data });
        }

        // GET: MVCExceptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mVCException = await _context.Exceptions
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mVCException == null)
            {
                return NotFound();
            }

            return View(mVCException);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleHandled(int id)
        {
            try
            {
                var table = await _context.Exceptions.FindAsync(id);

                table.Updated_at = DateTime.Now;
                table.Handled = DateTime.Now;

                _context.Update(table);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensagem = "Ok" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, mensagem = e.Message });
            }

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var table = await _context.Exceptions.FindAsync(id);

                table.Deleted_at = DateTime.Now;

                _context.Update(table);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensagem = "Ok" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, mensagem = e.Message });
            }
        }


        private bool MVCExceptionExists(int id)
        {
            return _context.Exceptions.Any(e => e.Id == id);
        }
    }
}