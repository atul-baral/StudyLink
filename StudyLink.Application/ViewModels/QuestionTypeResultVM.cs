using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class QuestionTypeResultVM
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public bool IsAnswered { get; set; }
    }
}
