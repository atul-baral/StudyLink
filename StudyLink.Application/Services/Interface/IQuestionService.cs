using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetList(int subjectId);
        Task<Question> GetById(int id);
        Task Add(AddQuestionVM question);
        Task Update(Question question);
        Task Delete(int id);
        Task<IEnumerable<AddAnswerVM>> GetListForAnswer(int subjectId);
        Task<IEnumerable<int>> GetQuestionTypeList(int subjectId);
        Task<int> GetQuestionCount(int subjectId, int questionTypeId);
    }
}
