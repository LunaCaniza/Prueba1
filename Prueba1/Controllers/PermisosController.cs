using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prueba1.Data;
using Prueba1.Models;
using Microsoft.AspNetCore.Authorization;
using Prueba1.Models;

namespace Prueba1.Controllers
{
    [Authorize]
    public class PermisosController : Controller
    {
        private readonly Prueba1Context _context;

        public PermisosController(Prueba1Context context)
        {
            _context = context;
        }

        // GET: Permisos
        public async Task<IActionResult> Index()
        {
            var mercoSys_ColegioCrecreContext = _context.Permisos.Include(p => p.Regla).Include(p => p.Usuario);
            return View(await mercoSys_ColegioCrecreContext.ToListAsync());
        }

        public async Task<IActionResult> IndexData(int draw, int start, int length)
        {
            string search;
            int orderColumn;
            string orderDir;
            string orderColumnName;

            #region Form
            orderColumn = Convert.ToInt32(Request.Form["order[0][column]"]);
            orderDir = Request.Form["order[0][dir]"];
            orderColumnName = Request.Form[$"columns[{orderColumn}][name]"];

            search = Request.Form["search[value]"];
            #endregion

            var query = _context.Permisos
                .Include(i => i.Usuario)
                .Include(i => i.Regla)
                .Where(w => !w.DeletadoEm.HasValue);

            int recordsTotal = query.Count(); //Logo após a query inicial

            #region Busca
            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                query = query.Where(w => EF.Functions.Like(w.Usuario.Nombre.ToUpper(), $"%{search}%") ||
                                         EF.Functions.Like(w.Regla.Regla.ToUpper(), $"%{search}%"));
            }
            #endregion
            #region Ordem
            if (orderColumnName.Contains("Usuario"))
            {
                orderColumnName = orderColumnName.Split(".")[1];

                if (orderDir == "asc")
                    query = query.OrderBy(o => EF.Property<object>(o.Usuario, orderColumnName));
                else
                    query = query.OrderByDescending(o => EF.Property<object>(o.Usuario, orderColumnName));

            }
            else if (orderColumnName.Contains("Regla"))
            {
                orderColumnName = orderColumnName.Split(".")[1];

                if (orderDir == "asc")
                    query = query.OrderBy(o => EF.Property<object>(o.Regla, orderColumnName));
                else
                    query = query.OrderByDescending(o => EF.Property<object>(o.Regla, orderColumnName));
            }
            else
            {
                if (orderDir == "asc")
                    query = query.OrderBy(o => EF.Property<object>(o, orderColumnName));
                else
                    query = query.OrderByDescending(o => EF.Property<object>(o, orderColumnName));
            }
            #endregion

            int recordsFiltered = query.Count(); //Logo após o Filtro
            var data = await query
                                .Skip(start)
                                .Take(length)
                                .ToListAsync();

            return Json(new { draw, recordsTotal, recordsFiltered, data });
        }

        // GET: Permisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos
                .Include(p => p.Regla)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permisos == null)
            {
                return NotFound();
            }

            return View(permisos);
        }

        // GET: Permisos/Create
        public IActionResult Create()
        {
            ViewData["ReglaId"] = new SelectList(_context.Reglas, "Id", "Regla");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Permisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,ReglaId,CreadoEm,ActualizadoEm,DeletadoEm")] Permisos permisos)
        {
            if (ModelState.IsValid)
            {
                permisos.CreadoEm = DateTime.Now;
                _context.Add(permisos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReglaId"] = new SelectList(_context.Reglas, "Id", "Regla", permisos.ReglaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", permisos.UsuarioId);
            return View(permisos);
        }

        // GET: Permisos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos.FindAsync(id);
            if (permisos == null)
            {
                return NotFound();
            }
            ViewData["ReglaId"] = new SelectList(_context.Reglas, "Id", "Id", permisos.ReglaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", permisos.UsuarioId);
            return View(permisos);
        }

        // POST: Permisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,ReglaId,CreadoEm,ActualizadoEm,DeletadoEm")] Permisos permisos)
        {
            if (id != permisos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permisos);
                    _context.Entry(permisos).Property(x => x.CreadoEm).IsModified = false;
                    permisos.ActualizadoEm = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisosExists(permisos.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReglaId"] = new SelectList(_context.Reglas, "Id", "Regla", permisos.ReglaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", permisos.UsuarioId);
            return View(permisos);
        }

        // GET: Permisos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos
                .Include(p => p.Regla)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permisos == null)
            {
                return NotFound();
            }

            return View(permisos);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var table = await _context.Permisos.FindAsync(id);

                table.DeletadoEm = DateTime.Now;

                _context.Update(table);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensagem = "Ok" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, mensagem = e.Message });
            }
        }

        private bool PermisosExists(int id)
        {
            return _context.Permisos.Any(e => e.Id == id);
        }
    }
}