using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyLink.Application.Services.Implementation
{
    internal class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Subject>> GetList()
        {
            return await _unitOfWork.Subjects.GetAllAsync();
        }

        public async Task<Subject> GetById(int id)
        {
            return await _unitOfWork.Subjects.GetAsync(u => u.SubjectId == id);
        }

        public async Task Add(Subject subject)
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();

            subject.SubjectQuestionTypes = questionTypes.Select(questionType => new SubjectQuestionType
            {
                SubjectId = subject.SubjectId,
                QuestionTypeId = questionType.QuestionTypeId
            }).ToList();

            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Update(Subject subject)
        {
            await _unitOfWork.Subjects.UpdateAsync(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id)
        {
            var subject = await _unitOfWork.Subjects.GetAsync(u => u.SubjectId == id);
            if (subject != null)
            {
                await _unitOfWork.Subjects.DeleteAsync(subject);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception($"Subject with Id {id} not found");
            }
        }

        public async Task<List<Subject>> GetListByUserId(string userId)
        {
            var student = await _unitOfWork.Students.GetAsync(
                s => s.UserId == userId,
                includeProperties: "StudentSubjects.Subject"
            );

            if (student != null)
            {
                return student.StudentSubjects
                    .Where(ss => !ss.IsDeleted)
                    .Select(ss => ss.Subject)
                    .ToList();
            }

            var teacher = await _unitOfWork.Teachers.GetAsync(
                t => t.UserId == userId,
                includeProperties: "TeacherSubjects.Subject"
            );

            if (teacher != null)
            {
                return teacher.TeacherSubjects
                    .Where(ts => !ts.IsDeleted)
                    .Select(ts => ts.Subject)
                    .ToList();
            }
            return new List<Subject>();
        }

    }
}
