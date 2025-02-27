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
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionController(IQuestionService questionService, IQuestionTypeService questionTypeService, ITeacherService teacherService, UserManager<ApplicationUser> userManager)
        {
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _teacherService = teacherService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                if ( id > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", id.ToString());
                }
                int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
                int questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var questions = await _questionService.GetAllQuestionsAsync( subjectId, questionTypeId);
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
            return View(questionTypes);
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
                    string subjectId = HttpContext.Session.GetString("SubjectId");
                    string questionTypeId = HttpContext.Session.GetString("QuestionTypeId");
                    var user = await _userManager.GetUserAsync(User);
                    int teacherId = (int)await _teacherService.GetTeacherIdByUserIdAsync(user.Id);
                    question.TeacherId = teacherId;
                    question.SubjectId = int.Parse(subjectId);
                    question.QuestionTypeId = int.Parse(questionTypeId);
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
    }
}
