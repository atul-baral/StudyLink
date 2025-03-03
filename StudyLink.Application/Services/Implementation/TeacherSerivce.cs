using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyLink.Application.Services.Implementation
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<AddTeacherVM>> GetAllTeachersAsync()
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync();
            var teacherVMs = new List<AddTeacherVM>();

            foreach (var teacher in teachers)
            {
                var user = await _userManager.FindByIdAsync(teacher.UserId);
                if (user != null)
                {
                    teacherVMs.Add(new AddTeacherVM
                    {
                        TeacherId = teacher.TeacherId,
                        UserId = teacher.UserId,
                        IsDeleted = teacher.IsDeleted,
                        TeacherSubjects = teacher.TeacherSubjects,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        Email = user.Email
                    });
                }
            }
            return teacherVMs;
        }


        public async Task<AddTeacherVM> GetTeacherByIdAsync(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(u => u.TeacherId == id, includeProperties: "TeacherSubjects");
            var user = await _userManager.FindByIdAsync(teacher.UserId);

            return new AddTeacherVM
            {
                TeacherId = teacher.TeacherId,
                UserId = teacher.UserId,
                IsDeleted = teacher.IsDeleted,
                TeacherSubjects = teacher.TeacherSubjects,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email
            };
        }

        public async Task AddTeacherAsync(AddTeacherVM teacher)
        {
            foreach (var teacherSubject in teacher.TeacherSubjects)
            {
                teacherSubject.CreatedAt = DateTime.Now;
            }

            var newUser = new ApplicationUser
            {
                UserName = teacher.Email,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Address = teacher.Address,
                Email = teacher.Email
            };

            var result = await _userManager.CreateAsync(newUser, "Teacher@1234");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user.");
            }
            await _userManager.AddToRoleAsync(newUser, "Teacher");

            var newTeacher = new Teacher
            {
                UserId = newUser.Id,
                IsDeleted = teacher.IsDeleted,
                TeacherSubjects = teacher.TeacherSubjects
            };

            await _unitOfWork.Teachers.AddAsync(newTeacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTeacherAsync(AddTeacherVM teacher)
        {
            var existingTeacher = await _unitOfWork.Teachers.GetAsync(s => s.TeacherId == teacher.TeacherId, includeProperties: "TeacherSubjects");

            var submittedSubjectIds = teacher.TeacherSubjects.Select(ss => ss.SubjectId).ToList();

            // Restore subjects that were marked as deleted but are in the submitted list
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

            // Add new subjects that are in the submitted list but not in the existing list
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


            // Update the associated ApplicationUser
            var user = await _userManager.FindByIdAsync(existingTeacher.UserId);
            if (user != null)
            {
                user.FirstName = teacher.FirstName;
                user.LastName = teacher.LastName;
                user.Address = teacher.Address;
                user.Email = teacher.Email;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user.");
                }
            }

            await _unitOfWork.Teachers.UpdateAsync(existingTeacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTeacherAsync(int id)
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

        public async Task<Teacher> GetTeacherByUserIdForSubjectsAsync(string userId)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(
                           t => t.UserId == userId,
                           includeProperties: "TeacherSubjects.Subject");

            if (teacher != null)
            {
                teacher.TeacherSubjects = teacher.TeacherSubjects.Where(ts => !ts.IsDeleted).ToList();
            }

            return teacher;
        }

        public async Task<int?> GetTeacherIdByUserIdAsync(string userId)
        {
            var teacher = await _unitOfWork.Teachers.GetAsync(t => t.UserId == userId);
            return teacher?.TeacherId;  
        }
    }
}
