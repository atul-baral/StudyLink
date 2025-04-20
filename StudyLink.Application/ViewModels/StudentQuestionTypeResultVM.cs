using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class StudentQuestionTypeResultVM
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public string SubjectName { get; set; }
        public List<StudentResultVM> StudentResults { get; set; }
    }

    public class StudentResultVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int FullMarks { get; set; }
        public int PassMarks { get; set; }
        public int MarksObtained { get; set; }
        public bool IsPass { get; set; }
    }
}
