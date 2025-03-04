using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class AddAnswerVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public ICollection<ChoiceVM> Choices { get; set; }

        public AnswerVM Answer { get; set; } = new AnswerVM();
    }

    public class ChoiceVM
    {
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; }
    }

    public class AnswerVM
    {
        public int ChoiceId { get; set; }  
    }
}
