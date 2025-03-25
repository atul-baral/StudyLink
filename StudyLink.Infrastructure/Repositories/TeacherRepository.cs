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
    internal class TeacherRepository: Repository<Teacher>, ITeacherRepository
    {
        private readonly StudyLinkDbContext _context;

        public TeacherRepository(StudyLinkDbContext context): base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            try
            {
                _context.Teachers.Update(teacher);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
