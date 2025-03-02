using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GetAllAnswersAsync();
        Task<Answer> GetAnswerByIdAsync(int id);
        Task AddAnswersAsync(IEnumerable<Answer> answers);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(int id);
        Task<int> GetCorrectAnswersAsync(int subjectId, int questionTypeId);
        Task<bool> HasStudentAnsweredAsync(int subjectId, int questionTypeId, int studentId);
    }
}
