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

    public class UsuariosController : Controller
    {
        private readonly Prueba1Context _context;

        public UsuariosController(Prueba1Context context)
        {
            _context = context;
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

            var query = _context.Usuarios
                .Where(w => !w.DeletadoEm.HasValue);

            int recordsTotal = query.Count(); //Logo após a query inicial

            #region Busca
            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                query = query.Where(w => EF.Functions.Like(w.Nombre.ToUpper(), $"%{search}%") ||
                                         EF.Functions.Like(w.Email.ToUpper(), $"%{search}%"));
            }
            #endregion
            #region Ordem            
            if (orderDir == "asc")
                query = query.OrderBy(o => EF.Property<object>(o, orderColumnName));
            else
                query = query.OrderByDescending(o => EF.Property<object>(o, orderColumnName));
            #endregion

            int recordsFiltered = query.Count(); //Logo após o Filtro
            var data = await query
                                .Skip(start)
                                .Take(length)
                                .ToListAsync();

            return Json(new { draw, recordsTotal, recordsFiltered, data });
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View();
        }

        //GET: AgregarProdutos
        public async Task<IActionResult> AgregarReglas(int id)
        {
            var usuario = await _context.Usuarios
                                           .Include(i => i.Permisos.Where(w => !w.DeletadoEm.HasValue))
                                                .ThenInclude(t => t.Regla)
                                           .FirstOrDefaultAsync(w => w.Id == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }


        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .Include(i => i.Permisos)
                    .ThenInclude(i => i.Regla)
                        .ThenInclude(i => i.Max)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Contrasena,CreadoEm,ActualizadoEm,DeletadoEm")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {

                usuarios.Contrasena = Usuarios.GetSha1(usuarios.Contrasena);
                usuarios.CreadoEm = DateTime.Now;
                _context.Add(usuarios);
                await _context.SaveChangesAsync();

                return RedirectToAction("AgregarReglas", new { id = usuarios.Id });
            }
            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,Contrasena,CreadoEm,ActualizadoEm,DeletadoEm")] Usuarios usuarios)
        {
            if (id != usuarios.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    _context.Entry(usuarios).Property(x => x.CreadoEm).IsModified = false;
                    usuarios.ActualizadoEm = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.Id))
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
            return View(usuarios);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var table = await _context.Usuarios.FindAsync(id);

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

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}