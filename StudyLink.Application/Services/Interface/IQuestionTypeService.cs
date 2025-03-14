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
        Task<IEnumerable<QuestionType>> GetList();
        Task<QuestionType> GetById(int id);
        Task Add(QuestionType questionType);
        Task Update(QuestionType questionType);
        Task Delete(int id);
        Task UpdateOrder(List<QuestionType> updatedQuestionTypes);
        Task TogglePublishStatus(int questionTypeId, bool isActive);
        Task<IEnumerable<QuestionType>> GetPublishedListBySubjectId(int subjectId);
    }

}
