using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IStudentRepository Students { get; }
        ISubjectRepository Subjects { get; }
        ITeacherRepository Teachers { get; }
        IStudentSubjectRepository StudentSubjects { get; }
        ITeacherSubjectRepository TeacherSubjects { get; }
        IQuestionTypeRepository QuestionTypes { get; }
        Task<int> CompleteAsync(); 
    }
}
