using Microsoft.AspNetCore.Http;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddQuestionAsync(AddQuestionVM addQuestionVm)
        {
            var question = new Question
            {
                QuestionText = addQuestionVm.QuestionText,
                SubjectId = addQuestionVm.SubjectId ?? 0,
                TeacherId = addQuestionVm.TeacherId ?? 0,
                QuestionTypeId = addQuestionVm.QuestionTypeId ?? 0,
                IsDeleted = false,
                Choices = addQuestionVm.Choices?.Select(c => new Choice
                {
                    ChoiceText = c.ChoiceText,
                    IsCorrect = c.IsCorrect
                }).ToList() ?? new List<Choice>()  
            };

            await _unitOfWork.Questions.AddAsync(question);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
        }



        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetAsync(u => u.QuestionId == id);
            if (question != null)
            {
                await _unitOfWork.Questions.DeleteAsync(question);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync(int subjectId, int questionTypeId)
        {
           return await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId);
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _unitOfWork.Questions.GetAsync(u => u.QuestionId == id, includeProperties: "Choices");
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            await _unitOfWork.Questions.UpdateAsync(question);
            await _unitOfWork.CompleteAsync();
        }
    }
}
