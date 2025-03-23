using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Models;
using MvcRecordStore.Models.ViewModels;
using MvcRecordStore.Services;

namespace MvcRecordStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LabelsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly ILabelService _labelService;

        public LabelsController(StoreDbContext context, ILabelService labelService)
        {
            _context = context;
            _labelService = labelService;
        }

        // GET: Labels
        public async Task<IActionResult> Index()
        {
            return View(_labelService.GetAllLabels());
        }

        // GET: Labels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var label = await _labelService.GetLabelWithDependencies((int)id);
            if (label == null)
            {
                return NotFound();
            }

            return View(label);
        }

        // GET: Labels/Create
        [Route("Admin/Labels/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Labels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Admin/Labels/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Country")] LabelCreateVM labelVM)
        {
            if (!ModelState.IsValid)
            {
                return View(labelVM);
            }

            var label = _labelService.CreateNewLabel(labelVM);
            _context.Add(label);
            await _context.SaveChangesAsync();
            return RedirectToAction("Labels", "Admin");
        }

        // GET: Labels/Edit/5
        [Route("Admin/Labels/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var label = await _labelService.GetLabelWithDependencies((int)id);
            if (label == null)
            {
                return NotFound();
            }
            
            var labelVM = _labelService.GetLabelViewModelToEdit(label);

            return View(labelVM);
        }

        // POST: Labels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Admin/Labels/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Country")] LabelCreateVM labelVM)
        {
            if (id != labelVM.ID)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = labelVM.ID });
            }

            var label = await _labelService.GetLabelWithDependencies(id);
            try
            {
                _context.Update(label);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_labelService.LabelExists(label.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Labels", "Admin");
        }

        // GET: Labels/Delete/5
        [Route("Admin/Labels/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var label = await _labelService.GetLabelWithoutDependencies((int)id);
            if (label == null)
            {
                return NotFound();
            }

            return View(label);
        }

        // POST: Labels/Delete/5
        [Route("Admin/Labels/Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var label = await _labelService.GetLabelWithoutDependencies((int)id);
            if (label != null)
            {
                _context.Labels.Remove(label);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Labels", "Admin");
        }
    }
}
