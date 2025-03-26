using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class QuestionType
    {
        public int QuestionTypeId { get; set; }
        public string TypeName { get; set; }
        public int SortOrder { get; set; }
        public int FullMarks { get; set; }
        public int PassMarks { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        [JsonIgnore]
        public ICollection<SubjectQuestionType> SubjectQuestionTypes { get; set; }
    }
}
