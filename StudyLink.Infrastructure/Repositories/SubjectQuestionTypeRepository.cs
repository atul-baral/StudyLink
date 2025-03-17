using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudyLink.Application.Interfaces;
using StudyLink.Domain.Entities;
using StudyLink.Infrastructure.Repository;

namespace StudyLink.Infrastructure.Repositories
{
    public class SubjectQuestionTypeRepository: Repository<SubjectQuestionType>, ISubjectQuestionTypeRepository
    {
        private readonly StudyLinkDbContext _context;
        public SubjectQuestionTypeRepository(StudyLinkDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task UpdateAsync(SubjectQuestionType subjectQuestionType)
        {
            _context.SubjectQuestionTypes.Update(subjectQuestionType);
        }
    }
}
