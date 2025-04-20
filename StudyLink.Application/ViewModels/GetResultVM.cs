using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Application.ViewModels
{
    public class GetResultVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Marks { get; set; }
        public ICollection<ResultChoiceVM> Choices { get; set; }

        public AnswerVM Answer { get; set; } = new AnswerVM();
    }

    public class ResultChoiceVM : ChoiceVM
    {
        public bool IsCorrect { get; set; }
    }
}
