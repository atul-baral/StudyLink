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
    internal class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        private readonly StudyLinkDbContext _context;

        public AnswerRepository(StudyLinkDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Answer answer)
        {
             _context.Answers.Update(answer);
        }
    }
}
