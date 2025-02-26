using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IStudentService
    {
        Task<IEnumerable<AddStudentVM>> GetAllStudentsAsync();
        Task<AddStudentVM> GetStudentByIdAsync(int id);
        Task AddStudentAsync(AddStudentVM student);
        Task UpdateStudentAsync(AddStudentVM student);
        Task DeleteStudentAsync(int id);
        Task<Student> GetStudentByUserIdAsync(string userId);
    }
}
