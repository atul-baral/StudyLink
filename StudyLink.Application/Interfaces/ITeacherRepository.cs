using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher> 
    {
        Task UpdateAsync(Teacher teacher);
    }
}
