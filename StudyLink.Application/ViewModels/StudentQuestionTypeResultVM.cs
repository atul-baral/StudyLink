using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class StudentQuestionTypeResultVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public string QuestionTypeName { get; set; }
        public string SubjectName { get; set; }
    }

}
