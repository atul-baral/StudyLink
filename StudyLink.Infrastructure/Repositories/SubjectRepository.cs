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
    internal class SubjectRepository: Repository<Subject>, ISubjectRepository
    {
        private readonly StudyLinkDbContext _context;

        public SubjectRepository(StudyLinkDbContext context): base(context) 
        {
            _context = context;
        }

        public async Task UpdateAsync(Subject subject)
        {

            try
            {
                _context.Subjects.Update(subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
