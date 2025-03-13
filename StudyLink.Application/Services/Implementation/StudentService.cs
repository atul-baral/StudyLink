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
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddStudentVM>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.Students.GetAllAsync(includeProperties: "StudentSubjects,User");
            var studentVMs = new List<AddStudentVM>();

            foreach (var student in students)
            {
                var addStudentVm = _mapper.Map<AddStudentVM>(student);
                studentVMs.Add(addStudentVm);
            }
            return studentVMs;
        }

        public async Task<AddStudentVM> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetAsync(u => u.StudentId == id, includeProperties: "StudentSubjects,User");
            return  _mapper.Map<AddStudentVM>(student);
        }

        public async Task AddStudentAsync(AddStudentVM studentVM)
        {
            foreach (var studentSubject in studentVM.StudentSubjects)
            {
                studentSubject.CreatedAt = DateTime.Now;
            }

            var newUser = _mapper.Map<ApplicationUser>(studentVM);

            var result = await _userManager.CreateAsync(newUser, "User@1234");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user.");
            }
            await _userManager.AddToRoleAsync(newUser, "Student");

            var newStudent = _mapper.Map<Student>(studentVM);
            newStudent.UserId = newUser.Id;

            await _unitOfWork.Students.AddAsync(newStudent);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateStudentAsync(AddStudentVM student)
        {
            var existingStudent = await _unitOfWork.Students.GetAsync(s => s.StudentId == student.StudentId, includeProperties: "StudentSubjects");

            var submittedSubjectIds = student.StudentSubjects.Select(ss => ss.SubjectId).ToList();

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

            var user = await _userManager.FindByIdAsync(existingStudent.UserId);
            if (user != null)
            {
                _mapper.Map(student, user);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user.");
                }
            }

            _mapper.Map(student, existingStudent);
            //existingStudent.UserId = user.Id;
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

        public async Task<Student> GetStudentByUserIdForSubjectsAsync(string userId)
        {
            var student = await _unitOfWork.Students.GetAsync(
                t => t.UserId == userId,
                includeProperties: "StudentSubjects.Subject");

            if (student != null)
            {
                student.StudentSubjects = student.StudentSubjects.Where(ss => !ss.IsDeleted).ToList();
            }

            return student;
        }

        public async Task<int> GetStudentIdByUserIdAsync(string userId)
        {
            var student = await _unitOfWork.Students.GetAsync(t => t.UserId == userId);
            return student?.StudentId ?? 0; 
        }

        public async Task<IEnumerable<Student>> GetStudentListBySubjectId(int subjectId)
        {
            return await _unitOfWork.Students.GetAllAsync(
                s => s.StudentSubjects.Any(ss => ss.SubjectId == subjectId),
                includeProperties: "User"
            );
        }

    }
}
