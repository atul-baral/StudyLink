using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.Interfaces;
using StudyLink.Domain.Entities;
using StudyLink.Infrastructure.Repository;

namespace StudyLink.Infrastructure.Repositories
{
    public class QuestionTypeRepository : Repository<QuestionType>, IQuestionTypeRepository
    {
        private readonly StudyLinkDbContext _context;

        public QuestionTypeRepository(StudyLinkDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(QuestionType questionType)
        {
            try
            {
                _context.QuestionTypes.Update(questionType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
