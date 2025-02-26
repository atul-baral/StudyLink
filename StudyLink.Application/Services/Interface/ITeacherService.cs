using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface ITeacherService
    {
        Task<IEnumerable<AddTeacherVM>> GetAllTeachersAsync();
        Task<AddTeacherVM> GetTeacherByIdAsync(int id);
        Task AddTeacherAsync(AddTeacherVM teacherSubject);
        Task UpdateTeacherAsync(AddTeacherVM teacherSubject);
        Task DeleteTeacherAsync(int id);
        Task<Teacher> GetTeacherByUserIdAsync(string userId);
    }
}
