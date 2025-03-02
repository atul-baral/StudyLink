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

        public async Task<IEnumerable<int>> GetDistinctQuestionTypeIdsAsync(int subjectId)
        {
            var questions = await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && !q.IsDeleted);
            return questions.Select(q => q.QuestionTypeId).Distinct();
        }

        public async Task<int> GetTotalQuestionsByTypeAsync(int subjectId, int questionTypeId)
        {
            var questions = await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId);
            return questions.Count();
        }

    }
}
