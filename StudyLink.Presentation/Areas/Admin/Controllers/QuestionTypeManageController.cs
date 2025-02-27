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
    public class QuestionTypeManageController : Controller
    {
        private readonly IQuestionTypeService _questionTypeService;

        public QuestionTypeManageController(IQuestionTypeService questionTypeService)
        {
            _questionTypeService = questionTypeService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var questionTypes = await _questionTypeService.GetAllQuestionTypesAsync();
                return View(questionTypes);
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
        public async Task<IActionResult> Create(QuestionType questionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _questionTypeService.AddQuestionTypeAsync(questionType);
                    TempData["Success"] = "QuestionType created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(questionType);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var questionType = await _questionTypeService.GetQuestionTypeByIdAsync(id);
                if (questionType == null)
                {
                    return NotFound();
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionType questionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _questionTypeService.UpdateQuestionTypeAsync(questionType);
                    TempData["Success"] = "QuestionType updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(questionType);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(QuestionType questionType)
        {
            try
            {
                await _questionTypeService.DeleteQuestionTypeAsync(questionType.QuestionTypeId);
                TempData["Success"] = "QuestionType deleted successfully!";
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
