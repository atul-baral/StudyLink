using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetList(int questionTypeId);
        Task<Question> GetById(int id);
        Task<int> Add(List<AddQuestionVM> addQuestionVm);
        Task<int> Update(Question question);
        Task Delete(int id);
        Task<IEnumerable<AddAnswerVM>> GetListForAnswer(int questionTypeId);
        Task<IEnumerable<int>> GetQuestionTypeList(int subjectId);
        Task<int> GetQuestionCount(int subjectId, int questionTypeId);
        Task<List<AddQuestionVM>> GetListForAddQuestion(int questionTypeId);
    }
}
