using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(Choice))]
        public int SelectedChoiceId { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public Student Student { get; set; }

        [ValidateNever]
        public Question Question { get; set; }

        [ValidateNever]
        public Choice SelectedChoice { get; set; }
    }
}

