using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;
using StudyLink.Presentation.Helpers;
using System;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubjectManageController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectManageController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return View(subjects);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _subjectService.AddSubjectAsync(subject);
                    TempData["Success"] = "Subject created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(subject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(subject);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return NotFound();
                }
                return View(subject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _subjectService.UpdateSubjectAsync(subject);
                    TempData["Success"] = "Subject updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(subject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(subject);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Subject subject)
        {
            try
            {
                await _subjectService.DeleteSubjectAsync(subject.SubjectId);
                TempData["Success"] = "Subject deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
