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
    public class TeacherSubjectRepository: Repository<TeacherSubject>, ITeacherSubjectRepository
    {
        private readonly StudyLinkDbContext _context;

        public TeacherSubjectRepository(StudyLinkDbContext context): base(context) 
        {
            _context = context;
        }

        public async Task UpdateAsync(TeacherSubject teacherSubject)
        {
            try
            {
                _context.TeacherSubjects.Update(teacherSubject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
