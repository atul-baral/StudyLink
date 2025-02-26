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
    public class StudentSubjectRepository: Repository<StudentSubject>, IStudentSubjectRepository
    {
        private readonly StudyLinkDbContext _context;

        public StudentSubjectRepository(StudyLinkDbContext context): base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(StudentSubject studentSubject)
        {
            try
            {
                _context.StudentSubjects.Update(studentSubject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
