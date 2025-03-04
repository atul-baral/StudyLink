using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync(int id);
        Task<Question> GetQuestionByIdAsync(int id);
        Task AddQuestionAsync(AddQuestionVM question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int id);
        Task<IEnumerable<AddAnswerVM>> GetAllQuestionsForAnswerAsync(int id);
    }
}
