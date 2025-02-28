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
        public List<ChoiceViewModel> Choices { get; set; }

        public AnswerViewModel Answer { get; set; } = new AnswerViewModel();
    }

    public class ChoiceViewModel
    {
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; }
    }

    public class AnswerViewModel
    {
        public int ChoiceId { get; set; }  
    }
}
