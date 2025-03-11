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
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();

            return questionTypes.OrderByDescending(x => x.SortOrder);
        }

        public async Task<QuestionType> GetQuestionTypeByIdAsync(int id)
        {
            return await _unitOfWork.QuestionTypes.GetAsync(u => u.QuestionTypeId == id);
        }

        public async Task AddQuestionTypeAsync(QuestionType questionType)
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();
            var maxSortOrder = questionTypes.OrderByDescending(q => q.SortOrder).FirstOrDefault();
            questionType.SortOrder = maxSortOrder != null ? maxSortOrder.SortOrder + 1 : 1;
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

        public async Task UpdateOrderAsync(List<QuestionType> updatedQuestionTypes)
        {
            foreach (var questionType in updatedQuestionTypes)
            {
                var existingQuestionType = await _unitOfWork.QuestionTypes.GetAsync(u => u.QuestionTypeId == questionType.QuestionTypeId);
                if (existingQuestionType != null)
                {
                    existingQuestionType.SortOrder = questionType.SortOrder;
                    _unitOfWork.QuestionTypes.UpdateAsync(existingQuestionType);
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task TogglePublishStatusAsync(int questionTypeId, bool isPublished)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(u => u.QuestionTypeId == questionTypeId);
            if (questionType != null)
            {
                questionType.IsPublished = isPublished;
                _unitOfWork.QuestionTypes.UpdateAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
