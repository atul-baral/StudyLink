using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using StudyLink.Presentation.Helpers;
using System;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeacherManageController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ISubjectService _subjectService;

        public TeacherManageController(ITeacherService teacherService, ISubjectService subjectService)
        {
            _teacherService = teacherService;
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachersAsync();
                return View(teachers);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.SubjectList = new SelectList(await _subjectService.GetAllSubjectsAsync(), "SubjectId", "SubjectName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddTeacherVM teacher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _teacherService.AddTeacherAsync(teacher);
                    TempData["Success"] = "Teacher created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(teacher);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ViewBag.SubjectList = new SelectList(await _subjectService.GetAllSubjectsAsync(), "SubjectId", "SubjectName");
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }

                teacher.TeacherSubjects = teacher.TeacherSubjects.Where(ss => !ss.IsDeleted).ToList();

                return View(teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddTeacherVM teacher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _teacherService.UpdateTeacherAsync(teacher);
                    TempData["Success"] = "Teacher updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(teacher);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(AddTeacherVM teacher)
        {
            try
            {
                await _teacherService.DeleteTeacherAsync(teacher.TeacherId);
                TempData["Success"] = "Teacher deleted successfully!";
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
