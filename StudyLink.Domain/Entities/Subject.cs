using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyLink.Domain.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        public string SubjectName { get; set; }

        [Required]
        public string SubjectCode { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ValidateNever]
        public ICollection<StudentSubject> StudentSubjects { get; set; }
        [ValidateNever]
        public ICollection<TeacherSubject> TeacherSubjects { get; set; }
    }
}
