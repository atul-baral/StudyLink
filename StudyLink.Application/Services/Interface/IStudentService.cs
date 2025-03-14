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
        Task<IEnumerable<AddStudentVM>> GetList();
        Task<AddStudentVM> GetById(int id);
        Task Add(AddStudentVM student);
        Task Update(AddStudentVM student);
        Task Delete(int id);
        Task<int> GetIdByUserId(string userId);
        Task<IEnumerable<Student>> GetListBySubjectId(int subjectId);
    }
}
