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
    public class StudentManageController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;

        public StudentManageController(IStudentService studentService, ISubjectService subjectService)
        {
            _studentService = studentService;
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var students = await _studentService.GetList();
                return View(students);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.SubjectList = new SelectList(await _subjectService.GetList(), "SubjectId", "SubjectName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddStudentVM student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _studentService.Add(student);
                    TempData["Success"] = "Student created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(student);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ViewBag.SubjectList = new SelectList(await _subjectService.GetList(), "SubjectId", "SubjectName");
                var student = await _studentService.GetById(id);
                if (student == null)
                {
                    return NotFound();
                }

                student.StudentSubjects = student.StudentSubjects.Where(ss => !ss.IsDeleted).ToList();

                return View(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddStudentVM student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _studentService.Update(student);
                    TempData["Success"] = "Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(student);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(AddStudentVM student)
        {
            try
            {
                await _studentService.Delete(student.StudentId);
                TempData["Success"] = "Student deleted successfully!";
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
