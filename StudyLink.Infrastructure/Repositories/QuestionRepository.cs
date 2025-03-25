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
    internal class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly StudyLinkDbContext _context;

        public QuestionRepository(StudyLinkDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
        }
    }
}
