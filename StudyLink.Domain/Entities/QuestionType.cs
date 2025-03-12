using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLink.Domain.Entities
{
    public class QuestionType
    {
        public int QuestionTypeId { get; set; }
        public string TypeName { get; set; }
        public int SortOrder { get; set; }
        public int FullMarks { get; set; }
        public int PassMarks { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
