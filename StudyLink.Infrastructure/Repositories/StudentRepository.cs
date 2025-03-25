using Microsoft.EntityFrameworkCore;
using StudyLink.Application.Interfaces;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudyLink.Infrastructure.Repository;

namespace StudyLink.Infrastructure.Repositories
{
    internal class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly StudyLinkDbContext _context;

        public StudentRepository(StudyLinkDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task UpdateAsync(Student student)
        {
            try
            {
                _context.Students.Update(student);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
