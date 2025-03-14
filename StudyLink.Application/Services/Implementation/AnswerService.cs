using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Implementation
{
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentService _studentSerivce;
        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public AnswerService(
            IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IStudentService studentService,
            IMapper mapper,
            IQuestionService questionService,
            IQuestionTypeService questionTypeService,
            ISubjectService subjectService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _studentSerivce = studentService;
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _subjectService = subjectService;
        }

        public async Task Add(List<AddAnswerVM> addAnswersVM)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int studentId = (int)await _studentSerivce.GetIdByUserId(user.Id);
            var answers = _mapper.Map<IEnumerable<Answer>>(addAnswersVM);
            foreach (var answer in answers)
            {
                answer.StudentId = studentId;
            }
            await _unitOfWork.Answers.AddRangeAsync(answers);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id)
        {
            var answer = await _unitOfWork.Answers.GetAsync(u => u.AnswerId == id);
            if (answer != null)
            {
                await _unitOfWork.Answers.DeleteAsync(answer);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Answer>> GetList()
        {
            return await _unitOfWork.Answers.GetAllAsync();
        }

        public async Task<Answer> GetById(int id)
        {
            return await _unitOfWork.Answers.GetAsync(u => u.QuestionId == id, includeProperties: "Selected Choice");
        }

        public async Task Update(Answer answer)
        {
            await _unitOfWork.Answers.UpdateAsync(answer);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetCorrectAnswerCount(int subjectId, int questionTypeId)
        {
            return await _unitOfWork.Answers.CountAsync(
                a => a.Question.SubjectId == subjectId &&
                     a.Question.QuestionTypeId == questionTypeId &&
                     a.SelectedChoice.IsCorrect,
                includeProperties: "Question"
            );
        }

        public async Task<bool> HasAnswered(int subjectId, int questionTypeId, int studentId)
        {
            return await _unitOfWork.Answers.AnyAsync(
                a => a.Question.SubjectId == subjectId &&
                     a.Question.QuestionTypeId == questionTypeId &&
                     a.StudentId == studentId
            );
        }

        public async Task<List<QuestionTypeResultVM>> GetQuestionTypeResultList(int subjectId)
        {
            if (subjectId > 0)
            {
                _httpContextAccessor.HttpContext.Session.SetString("SubjectId", subjectId.ToString());
            }

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int studentId = await _studentSerivce.GetIdByUserId(user.Id);
            var questionTypes = await _questionTypeService.GetPublishedListBySubjectId(subjectId);
            var questionTypeResults = new List<QuestionTypeResultVM>();

            foreach (var questionType in questionTypes)
            {
                int totalQuestions = await _questionService.GetQuestionCount(subjectId, questionType.QuestionTypeId);
                int correctAnswerCount = await GetCorrectAnswerCount(subjectId, questionType.QuestionTypeId);
                bool hasAnswered = await HasAnswered(subjectId, questionType.QuestionTypeId, studentId);
                questionTypeResults.Add(new QuestionTypeResultVM
                {
                    QuestionTypeId = questionType.QuestionTypeId,
                    QuestionTypeName = questionType.TypeName,
                    TotalQuestions = totalQuestions,
                    TotalCorrectAnswers = correctAnswerCount,
                    IsAnswered = hasAnswered
                });
            }
            return questionTypeResults;
        }

        public async Task<StudentQuestionTypeResultVM> GetStudentResultByQuestionTypeId(int questionTypeId)
        {
            int subjectId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("SubjectId"));
            var subject = await _subjectService.GetById(subjectId);
            var questionType = await _questionTypeService.GetById(questionTypeId);
            var students = await _studentSerivce.GetListBySubjectId(subjectId);
            var studentResults = new List<StudentResultVM>();

            foreach (var student in students)
            {
                if (await HasAnswered(subjectId, questionType.QuestionTypeId, student.StudentId))
                {
                    var studentId = student.StudentId;
                    var studentName = $"{student.User.FirstName} {student.User.LastName}";
                    int totalQuestionCount = await _questionService.GetQuestionCount(subjectId, questionType.QuestionTypeId);
                    int correctAnswerCount = await GetCorrectAnswerCount(subjectId, questionType.QuestionTypeId);
                    studentResults.Add(new StudentResultVM
                    {
                        StudentId = studentId,
                        StudentName = studentName,
                        TotalQuestions = totalQuestionCount,
                        TotalCorrectAnswers = correctAnswerCount
                    });
                }
            }

            return new StudentQuestionTypeResultVM
            {
                QuestionTypeName = questionType.TypeName,
                SubjectName = subject.SubjectName,
                StudentResults = studentResults
            };
        }
    }
}
