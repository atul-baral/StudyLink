using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class TeacherSubject
    {
        [Key]
        public int TeacherSubjectId{ get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public virtual Subject Subject { get; set; }

        [ValidateNever]
        public virtual Teacher Teacher { get; set; }
    }
}
