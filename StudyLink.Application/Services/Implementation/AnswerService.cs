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
        private readonly IMapper _mapper;

        public AnswerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IStudentService studentService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _studentSerivce = studentService;
        }

        public async Task AddAnswersAsync(IEnumerable<AddAnswerVM> addAnswersVM)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int studentId = (int)await _studentSerivce.GetStudentIdByUserIdAsync(user.Id);
            var answers = _mapper.Map<IEnumerable<Answer>>(addAnswersVM);
            foreach (var answer in answers)
            {
                answer.StudentId = studentId;
            }
            await _unitOfWork.Answers.AddRangeAsync(answers);
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await _unitOfWork.Answers.GetAsync(u => u.AnswerId == id);
            if (answer != null)
            {
                await _unitOfWork.Answers.DeleteAsync(answer);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Answer>> GetAllAnswersAsync()
        {
            return await _unitOfWork.Answers.GetAllAsync();
        }

        public async Task<Answer> GetAnswerByIdAsync(int id)
        {
            return await _unitOfWork.Answers.GetAsync(u => u.QuestionId == id, includeProperties: "Selected Choice");
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            await _unitOfWork.Answers.UpdateAsync(answer);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<QuestionTypeResultVM>> GetAllQuestionTypeWithResult(int id)
        {
            if (id > 0)
            {
                _httpContextAccessor.HttpContext.Session.SetString("SubjectId", id.ToString());
            }

            int subjectId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("SubjectId"));
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int studentId = (int)await _studentSerivce.GetStudentIdByUserIdAsync(user.Id);

            var questionTypeIds = (await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && !q.IsDeleted))
                .Select(q => q.QuestionTypeId)
                .Distinct();

            var questionTypeResults = new List<QuestionTypeResultVM>();

            foreach (var questionTypeId in questionTypeIds)
            {
                var questionType = (await _unitOfWork.QuestionTypes
                    .GetAllAsync(u => u.QuestionTypeId == questionTypeId && u.IsPublished == true))
                    .OrderByDescending(x => x.SortOrder)
                    .FirstOrDefault();

                if (questionType == null)
                {
                    continue;
                }

                var totalQuestions = (await _unitOfWork.Questions
                    .GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId))
                    .Count();

                var correctAnswers = await _unitOfWork.Answers.CountAsync(
                    a => a.Question.SubjectId == subjectId &&
                         a.Question.QuestionTypeId == questionTypeId &&
                         a.SelectedChoice.IsCorrect,
                    includeProperties: "Question"
                );

                var isAnswered = await _unitOfWork.Answers.AnyAsync(
                    a => a.Question.SubjectId == subjectId &&
                         a.Question.QuestionTypeId == questionTypeId &&
                         a.StudentId == studentId
                );

                questionTypeResults.Add(new QuestionTypeResultVM
                {
                    QuestionTypeId = questionTypeId,
                    QuestionTypeName = questionType.TypeName,
                    TotalQuestions = totalQuestions,
                    TotalCorrectAnswers = correctAnswers,
                    IsAnswered = isAnswered
                });
            }

            return questionTypeResults;
        }

        public async Task<IEnumerable<StudentQuestionTypeResultVM>> GetAllStudentsQuestionTypeResults(int id)
        {
            int subjectId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("SubjectId"));

            var subject = await _unitOfWork.Subjects.GetAsync(s => s.SubjectId == subjectId);
            string subjectName = subject.SubjectName;

            var students = await _unitOfWork.Students.GetAllAsync(
                s => s.StudentSubjects.Any(ss => ss.SubjectId == subjectId),
                includeProperties: "User"
            );

            var studentResults = new List<StudentQuestionTypeResultVM>();

            foreach (var student in students)
            {
                bool hasAnswered = await _unitOfWork.Answers.AnyAsync(
                    a => a.StudentId == student.StudentId &&
                         a.Question.SubjectId == subjectId &&
                         a.Question.QuestionTypeId == id
                );

                if (hasAnswered)
                {
                    var studentId = student.StudentId;
                    var studentName = $"{student.User.FirstName} {student.User.LastName}";

                    var totalQuestions = (await _unitOfWork.Questions
                        .GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == id && !q.IsDeleted))
                        .Count();

                    var correctAnswers = await _unitOfWork.Answers.CountAsync(
                        a => a.Question.SubjectId == subjectId &&
                             a.Question.QuestionTypeId == id &&
                             a.StudentId == studentId &&
                             a.SelectedChoice.IsCorrect,
                        includeProperties: "Question"
                    );

                    studentResults.Add(new StudentQuestionTypeResultVM
                    {
                        StudentId = studentId,
                        StudentName = studentName,
                        TotalQuestions = totalQuestions,
                        TotalCorrectAnswers = correctAnswers,
                        QuestionTypeName = (await _unitOfWork.QuestionTypes.GetAsync(qt => qt.QuestionTypeId == id))?.TypeName,
                        SubjectName = subjectName
                    });
                }
            }

            return studentResults;
        }





    }
}
