using Microsoft.Extensions.DependencyInjection;
using StudyLink.Application.Interfaces;
using StudyLink.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace StudyLink.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudyLinkDbContext _context;
        public IStudentRepository Students { get; private set; }
        public ISubjectRepository Subjects { get; private set; }
        public ITeacherRepository Teachers { get; private set; }
        public IStudentSubjectRepository StudentSubjects { get; private set; }
        public ITeacherSubjectRepository TeacherSubjects { get; private set; }
        public IQuestionTypeRepository QuestionTypes { get; private set; }
        public IQuestionRepository Questions { get; private set; }
        public IChoiceRepository Choices { get; private set; }

        public UnitOfWork(StudyLinkDbContext context)
        {
            _context = context;
            Students = new StudentRepository(_context);
            Subjects = new SubjectRepository(_context);
            Teachers = new TeacherRepository(_context);
            StudentSubjects = new StudentSubjectRepository(_context);
            TeacherSubjects = new TeacherSubjectRepository(_context);
            QuestionTypes = new QuestionTypeRepository(_context);
            Questions = new QuestionRepository(_context);
            Choices = new ChoiceRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
