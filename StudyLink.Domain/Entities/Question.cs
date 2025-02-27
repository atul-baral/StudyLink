using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        [ForeignKey(nameof(QuestionType))]
        public int QuestionTypeId { get; set; }

        public string QuestionText { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public Subject Subject { get; set; }

        [ValidateNever]
        public Teacher Teacher { get; set; }

        [ValidateNever]
        public QuestionType QuestionType { get; set; }

        [JsonIgnore]
        public ICollection<Choice> Choices { get; set; }
    }
}

