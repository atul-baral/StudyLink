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

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsForAnswerAsync(id);

                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> ListQuestionTypesWithResult(int id=0)
        {
            var questionTypeWithResult = await _answerService.GetAllQuestionTypeWithResult(id);
            return View(questionTypeWithResult);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer(List<AddAnswerVM> addAnswersVm)
        {
            try
            {
                await _answerService.AddAnswersAsync(addAnswersVm);
                TempData["Success"] = "Answers Submitted successfully!";
                return RedirectToAction("ListQuestionTypesWithResult");
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }


    }
}
