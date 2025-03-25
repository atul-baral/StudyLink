using Microsoft.Extensions.DependencyInjection;
using StudyLink.Application.Interfaces;
using StudyLink.Domain.Entities;
using StudyLink.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace StudyLink.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly StudyLinkDbContext _context;
        public IStudentRepository Students { get; private set; }
        public ISubjectRepository Subjects { get; private set; }
        public ITeacherRepository Teachers { get; private set; }
        public IQuestionTypeRepository QuestionTypes { get; private set; }
        public IQuestionRepository Questions { get; private set; }
        public IAnswerRepository Answers { get; private set; }
        public ISubjectQuestionTypeRepository SubjectQuestionTypes { get; private set; }

        public UnitOfWork(StudyLinkDbContext context)
        {
            _context = context;
            Students = new StudentRepository(_context);
            Subjects = new SubjectRepository(_context);
            Teachers = new TeacherRepository(_context);
            QuestionTypes = new QuestionTypeRepository(_context);
            Questions = new QuestionRepository(_context);
            Answers = new AnswerRepository(_context);
            SubjectQuestionTypes = new SubjectQuestionTypeRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
