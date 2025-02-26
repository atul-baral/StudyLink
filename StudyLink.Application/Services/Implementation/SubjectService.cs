using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyLink.Application.Services.Implementation
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _unitOfWork.Subjects.GetAllAsync();
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _unitOfWork.Subjects.GetAsync(u => u.SubjectId == id);
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            await _unitOfWork.Subjects.UpdateAsync(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteSubjectAsync(int id)
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
    }
}
