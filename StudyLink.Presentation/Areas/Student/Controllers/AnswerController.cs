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

        public async Task<IActionResult> ListQuestionTypesWithResult(int id)
        {
            if (id > 0)
            {
                HttpContext.Session.SetString("SubjectId", id.ToString());
            }

            int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            int studentId = (int)await _studentService.GetStudentIdByUserIdAsync(user.Id);

            var questionTypeIds = await _questionTypeService.GetDistinctQuestionTypeIdsAsync(subjectId);
            var questionTypeResults = new List<QuestionTypeResultVM>();

            foreach (int questionTypeId in questionTypeIds)
            {
                var questionType = await _questionTypeService.GetQuestionTypeByIdAsync(questionTypeId);
                var totalQuestions = await _questionTypeService.GetTotalQuestionsByTypeAsync(subjectId, questionTypeId);
                var correctAnswers = await _answerService.GetCorrectAnswersAsync(subjectId, questionTypeId);
                var isAnswered = await _answerService.HasStudentAnsweredAsync(subjectId, questionTypeId, studentId);

                questionTypeResults.Add(new QuestionTypeResultVM
                {
                    QuestionTypeId = questionTypeId,
                    QuestionTypeName = questionType.TypeName,
                    TotalQuestions = totalQuestions,
                    TotalCorrectAnswers = correctAnswers,
                    IsAnswered = isAnswered
                });
            }

            return View(questionTypeResults);
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

                await _answerService.AddAnswersAsync(answers);
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
