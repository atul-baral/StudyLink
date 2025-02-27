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
        IQuestionTypeRepository QuestionTypes { get; }
        IQuestionRepository Questions { get; }
        IAnswerRepository Answers { get; }
        Task<int> CompleteAsync(); 
    }
}
