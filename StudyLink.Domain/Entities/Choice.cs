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
    public class Choice
    {
        [Key]
        public int ChoiceId { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        public string ChoiceText { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        [JsonIgnore]
        public Question Question { get; set; }
    }
}

