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
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<AddStudentVM>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync();
            var studentVMs = new List<AddStudentVM>();

            foreach (var student in students)
            {
                var user = await _userManager.FindByIdAsync(student.UserId);
                if (user != null)
                {
                    studentVMs.Add(new AddStudentVM
                    {
                        StudentId = student.StudentId,
                        UserId = student.UserId,
                        IsDeleted = student.IsDeleted,
                        StudentSubjects = student.StudentSubjects,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        Email = user.Email
                    });
                }
            }
            return studentVMs;
        }


        public async Task<AddStudentVM> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetAsync(u => u.StudentId == id, includeProperties: "StudentSubjects");
            var user = await _userManager.FindByIdAsync(student.UserId);

            return new AddStudentVM
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                IsDeleted = student.IsDeleted,
                StudentSubjects = student.StudentSubjects,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email
            };
        }

        public async Task AddStudentAsync(AddStudentVM student)
        {
            foreach (var studentSubject in student.StudentSubjects)
            {
                studentSubject.CreatedAt = DateTime.Now;
            }

            var newUser = new ApplicationUser
            {
                UserName = student.Email,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address = student.Address,
                Email = student.Email
            };

            var result = await _userManager.CreateAsync(newUser, "User@1234");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user.");
            }
            await _userManager.AddToRoleAsync(newUser, "Student");

            var newStudent = new Student
            {
                UserId = newUser.Id,
                IsDeleted = student.IsDeleted,
                StudentSubjects = student.StudentSubjects
            };

            await _unitOfWork.Students.AddAsync(newStudent);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateStudentAsync(AddStudentVM student)
        {
            var existingStudent = await _unitOfWork.Students.GetAsync(s => s.StudentId == student.StudentId, includeProperties: "StudentSubjects");

            var submittedSubjectIds = student.StudentSubjects.Select(ss => ss.SubjectId).ToList();

            // Restore subjects that were marked as deleted but are in the submitted list
            foreach (var existingSubject in existingStudent.StudentSubjects)
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
            foreach (var subjectId in submittedSubjectIds.Except(existingStudent.StudentSubjects.Select(es => es.SubjectId)))
            {
                existingStudent.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = student.StudentId,
                    SubjectId = subjectId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                });
            }


            // Update the associated ApplicationUser
            var user = await _userManager.FindByIdAsync(existingStudent.UserId);
            if (user != null)
            {
                user.FirstName = student.FirstName;
                user.LastName = student.LastName;
                user.Address = student.Address;
                user.Email = student.Email;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user.");
                }
            }

            await _unitOfWork.Students.UpdateAsync(existingStudent);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetAsync(u => u.StudentId == id);
            if (student != null)
            {
                await _unitOfWork.Students.DeleteAsync(student);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception($"Student with Id {id} not found");
            }
        }

        public async Task<Student> GetStudentByUserIdAsync(string userId)
        {
            return await _unitOfWork.Students.GetAsync(t => t.UserId == userId, includeProperties: "StudentSubjects.Subject");
        }
    }
}
