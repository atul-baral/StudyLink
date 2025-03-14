using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface ISubjectService
    {

        Task<IEnumerable<Subject>> GetList();
        Task<Subject> GetById(int id);
        Task Add(Subject subject);
        Task Update(Subject subject);
        Task Delete(int id);
        Task<List<Subject>> GetListByUserId(string userId);
    }
}
