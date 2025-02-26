using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class StudentSubject
    {
        [Key]
        public int StudentSubjectId { get; set; }

        [ForeignKey(nameof(Subject))] 
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Student))] 
        public int StudentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public virtual Subject Subject { get; set; }

        [ValidateNever]
        public virtual Student Student { get; set; }
    }
}
