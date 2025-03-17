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
        Task<IEnumerable<QuestionType>> GetListBySubjectId(int subjectId);
        Task<QuestionType> GetById(int id);
        Task Add(QuestionType questionType);
        Task Update(QuestionType questionType);
        Task Delete(int id);
        Task UpdateOrder(List<QuestionType> updatedQuestionTypes);
        Task<int> TogglePublishStatus(int questionTypeId,int subjectId, bool isPublished);
        Task<IEnumerable<QuestionType>> GetPublishedListBySubjectId(int subjectId);
        Task<int> GetQuestionMarksDifferenceFromFullMarks(int questionTypeId, int subjectId);
        Task<IEnumerable<SubjectQuestionType>> GetQuestionTypeDetails(int questionTypeId);
    }

}
