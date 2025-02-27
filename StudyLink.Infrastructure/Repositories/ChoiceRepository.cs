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
    public class ChoiceRepository : Repository<Choice>, IChoiceRepository
    {
        private readonly StudyLinkDbContext _context;

        public ChoiceRepository(StudyLinkDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Choice choice)
        {
            _context.Choices.Update(choice);
        }
    }
}
