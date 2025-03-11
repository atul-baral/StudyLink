using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using StudyLink.Presentation.Helpers;
using System;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IAnswerService _answerService;

        public QuestionController(IQuestionService questionService, IQuestionTypeService questionTypeService, IAnswerService answerService)
        {
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
        }

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync(id);
                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> QuestionTypes(int id)
        {
            HttpContext.Session.SetString("SubjectId", id.ToString());

            var questionTypes = await _questionTypeService.GetAllQuestionTypesAsync();
            var filteredAndOrderedQuestionTypes = questionTypes
                .OrderByDescending(x => x.SortOrder)
                .ToList();

            return View(filteredAndOrderedQuestionTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddQuestionVM question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _questionService.AddQuestionAsync(question);
                    TempData["Success"] = "Question created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(question);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _questionService.UpdateQuestionAsync(question);
                    TempData["Success"] = "Question updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(question);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Question question)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(question.QuestionId);
                TempData["Success"] = "Question deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetStudentResults(int id)
        {
            try
            {
                var questions = await _answerService.GetAllStudentsQuestionTypeResults(id);
                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }
    }
}
