﻿using Microsoft.AspNetCore.Authorization;
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
                var questionTypes = await _questionTypeService.GetList();
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
                    await _questionTypeService.Add(questionType);
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
                var questionType = await _questionTypeService.GetById(id);
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
                    await _questionTypeService.Update(questionType);
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
                await _questionTypeService.Delete(questionType.QuestionTypeId);
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

        [HttpPost]
        public async Task<IActionResult> UpdateOrder([FromBody] List<QuestionType> updatedQuestionTypes)
        {
            try
            {
                await _questionTypeService.UpdateOrder(updatedQuestionTypes);
                //return RedirectToAction(nameof(Index));
                return Json(new { success = true, message = "Sort order updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the sort order." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> TogglePublishStatus([FromBody] QuestionType questionType)
        {
            try
            {
                await _questionTypeService.TogglePublishStatus(questionType.QuestionTypeId, questionType.IsPublished);
                return Json(new { success = true, message = "Publish status updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the toggle publish." });
            }
        }

    }
}
