using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GetList();
        Task<Answer> GetById(int id);
        Task Add(List<AddAnswerVM> addAnswersVM);
        Task Update(Answer answer);
        Task Delete(int id);
        Task<int> GetCorrectAnswerCount(int subjectId, int questionTypeId);
        Task<bool> HasAnswered(int subjectId, int questionTypeId, int studentId);
        Task<List<QuestionTypeResultVM>> GetQuestionTypeResultList(int subjectId);
        Task<StudentQuestionTypeResultVM> GetStudentResultByQuestionTypeId(int questionTypeId);
        Task<List<GetResultVM>> GetResultAsync(int studentId);
    }
}
