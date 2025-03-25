using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace StudyLink.Application.Services.Implementation
{
    internal class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public TeacherService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddTeacherVM>> GetList()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(includeProperties: "TeacherSubjects,User");
            var teacherVMs = new List<AddTeacherVM>();

            foreach (var teacher in teachers)
            {
                var addTeacherVm = _mapper.Map<AddTeacherVM>(teacher);
                teacherVMs.Add(addTeacherVm);
            }
            return teacherVMs;
        }

        public async Task<AddTeacherVM> GetById(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(u => u.TeacherId == id, includeProperties: "TeacherSubjects,User");
            return _mapper.Map<AddTeacherVM>(teacher);
        }

        public async Task Add(AddTeacherVM teacherVM)
        {
            foreach (var teacherSubject in teacherVM.TeacherSubjects)
            {
                teacherSubject.CreatedAt = DateTime.Now;
            }

            var newUser = _mapper.Map<ApplicationUser>(teacherVM);

            var result = await _userManager.CreateAsync(newUser, "User@1234");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user.");
            }
            await _userManager.AddToRoleAsync(newUser, "Teacher");

            var newTeacher = _mapper.Map<Teacher>(teacherVM);
            newTeacher.UserId = newUser.Id;

            await _unitOfWork.Teachers.AddAsync(newTeacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Update(AddTeacherVM teacher)
        {
            var existingTeacher = await _unitOfWork.Teachers.GetAsync(s => s.TeacherId == teacher.TeacherId, includeProperties: "TeacherSubjects");

            var submittedSubjectIds = teacher.TeacherSubjects.Select(ss => ss.SubjectId).ToList();

            foreach (var existingSubject in existingTeacher.TeacherSubjects)
            {
                if (submittedSubjectIds.Contains(existingSubject.SubjectId) && existingSubject.IsDeleted)
                {
                    existingSubject.IsDeleted = false;
                }
                else if (!submittedSubjectIds.Contains(existingSubject.SubjectId))
                {
                    existingSubject.IsDeleted = true;
                }
            }

            foreach (var subjectId in submittedSubjectIds.Except(existingTeacher.TeacherSubjects.Select(es => es.SubjectId)))
            {
                existingTeacher.TeacherSubjects.Add(new TeacherSubject
                {
                    TeacherId = teacher.TeacherId,
                    SubjectId = subjectId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                });
            }

            var user = await _userManager.FindByIdAsync(existingTeacher.UserId);
            if (user != null)
            {
                _mapper.Map(teacher, user);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user.");
                }
            }

            _mapper.Map(teacher, existingTeacher);
            await _unitOfWork.Teachers.UpdateAsync(existingTeacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(u => u.TeacherId == id);
            if (teacher != null)
            {
                await _unitOfWork.Teachers.DeleteAsync(teacher);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception($"Teacher with Id {id} not found");
            }
        }

        public async Task<int> GetIdByUserId(string userId)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(s => s.UserId == userId);
            return teacher?.TeacherId ?? 0;
        }
    }
}
