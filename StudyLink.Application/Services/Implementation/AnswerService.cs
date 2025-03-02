using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Implementation
{
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnswerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAnswersAsync(IEnumerable<Answer> answers)
        {
            await _unitOfWork.Answers.AddRangeAsync(answers);
            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await _unitOfWork.Answers.GetAsync(u => u.AnswerId == id);
            if (answer != null)
            {
                await _unitOfWork.Answers.DeleteAsync(answer);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Answer>> GetAllAnswersAsync()
        {
            return await _unitOfWork.Answers.GetAllAsync();
        }

        public async Task<Answer> GetAnswerByIdAsync(int id)
        {
            return await _unitOfWork.Answers.GetAsync(u => u.QuestionId == id, includeProperties: "Selected Choice");
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            await _unitOfWork.Answers.UpdateAsync(answer);
            await _unitOfWork.CompleteAsync();
        }
    }
}
