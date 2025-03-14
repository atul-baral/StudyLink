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
        Task<IEnumerable<AddTeacherVM>> GetList();
        Task<AddTeacherVM> GetById(int id);
        Task Add(AddTeacherVM teacherSubject);
        Task Update(AddTeacherVM teacherSubject);
        Task Delete(int id);
        Task<int> GetIdByUserId(string userId);
    }
}
