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

    public class ProductosController : Controller
    {
        private readonly Prueba1Context _context;

        public ProductosController(Prueba1Context context)
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

            var query = _context.Productos
                .Where(w => !w.DeletadoEm.HasValue);

            int recordsTotal = query.Count(); //Logo após a query inicial

            #region Busca
            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToUpper();
                query = query.Where(w => EF.Functions.Like(w.Descripcion.ToUpper(), $"%{search}%"));
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

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Observacion,CreadoEm,ActualizadoEm,DeletadoEm")] Productos productos)
        {
            if (ModelState.IsValid)
            {
                productos.CreadoEm = DateTime.Now;
                _context.Add(productos);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(productos);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos.FindAsync(id);
            if (productos == null)
            {
                return NotFound();
            }
            return View(productos);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Observacion,CreadoEm,ActualizadoEm,DeletadoEm")] Productos productos)
        {
            if (id != productos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productos);
                    _context.Entry(productos).Property(x => x.CreadoEm).IsModified = false;
                    productos.ActualizadoEm = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductosExists(productos.Id))
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
            return View(productos);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var table = await _context.Productos.FindAsync(id);

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

        private bool ProductosExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}