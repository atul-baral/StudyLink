using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<QuestionType>> GetList()
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();
            return questionTypes.OrderByDescending(q => q.SortOrder);
        }

        public async Task<QuestionType> GetById(int id)
        {
            return await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == id);
        }

        public async Task Add(QuestionType questionType)
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();
            var maxSortOrder = questionTypes.OrderByDescending(q => q.SortOrder).FirstOrDefault();
            questionType.SortOrder = maxSortOrder != null ? maxSortOrder.SortOrder + 1 : 1;

            await _unitOfWork.QuestionTypes.AddAsync(questionType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Update(QuestionType questionType)
        {
            await _unitOfWork.QuestionTypes.UpdateAsync(questionType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == id);
            if (questionType != null)
            {
                await _unitOfWork.QuestionTypes.DeleteAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UpdateOrder(List<QuestionType> updatedQuestionTypes)
        {
            foreach (var questionType in updatedQuestionTypes)
            {
                var existingQuestionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionType.QuestionTypeId);
                if (existingQuestionType != null)
                {
                    existingQuestionType.SortOrder = questionType.SortOrder;
                    await _unitOfWork.QuestionTypes.UpdateAsync(existingQuestionType);
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task TogglePublishStatus(int questionTypeId, bool isActive)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionTypeId);
            if (questionType != null)
            {
                questionType.IsPublished = isActive;
                await _unitOfWork.QuestionTypes.UpdateAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<QuestionType>> GetPublishedListBySubjectId(int subjectId)
        {
            var questions = await _unitOfWork.Questions
                .GetAllAsync(q => q.SubjectId == subjectId && !q.IsDeleted, includeProperties: "QuestionType");

            var questionTypes = questions
                .Select(q => q.QuestionType)
                .Distinct()
                .Where(qt => qt.IsPublished && !qt.IsDeleted)
                .OrderByDescending(qt => qt.SortOrder)
                .ToList();

            return questionTypes;
        }
    }
}
