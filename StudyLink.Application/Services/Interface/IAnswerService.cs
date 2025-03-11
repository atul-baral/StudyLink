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
        Task<IEnumerable<Answer>> GetAllAnswersAsync();
        Task<Answer> GetAnswerByIdAsync(int id);
        Task AddAnswersAsync(IEnumerable<AddAnswerVM> addAnswersVM);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(int id);
        Task<IEnumerable<QuestionTypeResultVM>> GetAllQuestionTypeWithResult(int id);
        Task<StudentQuestionTypeResultVM> GetAllStudentsQuestionTypeResults(int id);
    }
}
