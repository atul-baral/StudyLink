using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyLink.Application.Services.Implementation
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync()
        {
            return await _unitOfWork.QuestionTypes.GetAllAsync();
        }

        public async Task<QuestionType> GetQuestionTypeByIdAsync(int id)
        {
            return await _unitOfWork.QuestionTypes.GetAsync(u => u.QuestionTypeId == id);
        }

        public async Task AddQuestionTypeAsync(QuestionType questionType)
        {
            await _unitOfWork.QuestionTypes.AddAsync(questionType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateQuestionTypeAsync(QuestionType questionType)
        {
            await _unitOfWork.QuestionTypes.UpdateAsync(questionType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteQuestionTypeAsync(int id)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(u => u.QuestionTypeId == id);
            if (questionType != null)
            {
                await _unitOfWork.QuestionTypes.DeleteAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception($"QuestionType with Id {id} not found");
            }
        }

        public async Task<IEnumerable<QuestionTypeResultVM>> GetQuestionTypeResultsAsync(int subjectId, int studentId)
        {
            var questions = await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && !q.IsDeleted);
            var questionTypeIds = questions.Select(q => q.QuestionTypeId).Distinct();

            var results = new List<QuestionTypeResultVM>();

            foreach (int qtId in questionTypeIds)
            {
                var questionType = await _unitOfWork.Questions.GetAsync(u => u.QuestionTypeId == qtId, includeProperties: "QuestionType");
                var totalQuestions = questions.Count(q => q.QuestionTypeId == qtId);

                var correctAnswers = await _unitOfWork.Answers.CountAsync(
                    a => a.Question.SubjectId == subjectId &&
                         a.Question.QuestionTypeId == qtId &&
                         a.SelectedChoice.IsCorrect,
                    includeProperties: "Question"
                );

                bool isAnswered = await _unitOfWork.Answers.AnyAsync(
                    a => a.Question.SubjectId == subjectId &&
                         a.Question.QuestionTypeId == qtId &&
                         a.StudentId == studentId
                );

                results.Add(new QuestionTypeResultVM
                {
                    QuestionTypeId = qtId,
                    QuestionTypeName = questionType.QuestionType.TypeName,
                    TotalQuestions = totalQuestions,
                    TotalCorrectAnswers = correctAnswers,
                    IsAnswered = isAnswered 
                });
            }

            return results;
        }

    }
}
