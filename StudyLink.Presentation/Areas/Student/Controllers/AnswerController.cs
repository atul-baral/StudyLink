using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Services.Implementation;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using StudyLink.Presentation.Helpers;

namespace StudyLink.Presentation.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;

        public AnswerController(IAnswerService answerService, IQuestionService questionService)
        {
            _answerService = answerService;
            _questionService = questionService;
        }

        public async Task<IActionResult> Index(int questionTypeId)
        {
            try
            {
                var questions = await _questionService.GetListForAnswer(questionTypeId);

                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> ListQuestionTypesWithResult(int id=0)
        {
            var questionTypeWithResult = await _answerService.GetQuestionTypeResultList(id);
            return View(questionTypeWithResult);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer(List<AddAnswerVM> addAnswersVm)
        {
            try
            {
                await _answerService.Add(addAnswersVm);
                TempData["Success"] = "Answers Submitted successfully!";
                return RedirectToAction("ListQuestionTypesWithResult");
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> GetResultDetail(int studentId, int questionTypeId)
        {
            try
            {
                if (questionTypeId > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", questionTypeId.ToString());
                }
                questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var result = await _answerService.GetResultAsync(studentId);
                return Json(new { success = true, data = result });
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
