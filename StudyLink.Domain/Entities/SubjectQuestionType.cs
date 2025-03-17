using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StudyLink.Domain.Entities
{
    public class SubjectQuestionType
    {
        public int SubjectQuestionTypeId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(QuestionType))]
        public int QuestionTypeId { get; set; }

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        [ValidateNever]
        public Subject Subject { get; set; }

        [ValidateNever]
        public QuestionType QuestionType { get; set; }
    }
}
