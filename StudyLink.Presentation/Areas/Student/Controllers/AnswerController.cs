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
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswerController(IAnswerService answerService, IQuestionService questionService, IQuestionTypeService questionTypeService, IStudentService studentService, UserManager<ApplicationUser> userManager)
        {
            _answerService = answerService;
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _studentService = studentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                if (id > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", id.ToString());
                }

                int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
                int questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));

                var questions = await _questionService.GetAllQuestionsAsync(subjectId, questionTypeId);

                var viewModel = questions.Select(q => new AddAnswerVM
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Choices = q.Choices.Select(c => new ChoiceViewModel
                    {
                        ChoiceId = c.ChoiceId,
                        ChoiceText = c.ChoiceText
                    }).ToList(),
                    Answer = new AnswerViewModel() 
                }).ToList();

                return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> AddAnswer(List<AddAnswerVM> model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                int studentId = (int)await _studentService.GetStudentIdByUserIdAsync(user.Id);
                var answers = model.Select(q => new Answer
                {
                    QuestionId = q.QuestionId,
                    SelectedChoiceId = q.Answer.ChoiceId,
                    StudentId = studentId
                }).ToList();

                foreach (var answer in answers)
                {
                    await _answerService.AddAnswerAsync(answer);
                }
                TempData["Success"] = "Answers Submitted successfully!";
                return RedirectToAction("QuestionTypes");
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

    }
}
