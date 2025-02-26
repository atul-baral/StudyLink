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
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Score { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public Student Student { get; set; }

        [ValidateNever]
        public Subject Subject { get; set; }
    }
}

