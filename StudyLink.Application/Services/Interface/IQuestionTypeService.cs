using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IQuestionTypeService
    {
        Task<IEnumerable<QuestionType>> GetAllQuestionTypesAsync();
        Task<QuestionType> GetQuestionTypeByIdAsync(int id);
        Task AddQuestionTypeAsync(QuestionType questionType);
        Task UpdateQuestionTypeAsync(QuestionType questionType);
        Task DeleteQuestionTypeAsync(int id);
        Task UpdateOrderAsync(List<QuestionType> updatedQuestionTypes);
        Task TogglePublishStatusAsync(int questionTypeId, bool isActive);
    }
}
