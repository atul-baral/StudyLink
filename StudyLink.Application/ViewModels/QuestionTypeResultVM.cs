using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class QuestionTypeResultVM
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public int MarksObtained { get; set; }
        public bool IsPass { get; set; }
        public bool IsAnswered { get; set; }
        public int StudentId { get; set; }
    }
}
