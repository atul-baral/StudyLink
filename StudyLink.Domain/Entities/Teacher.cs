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
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public ICollection<TeacherSubject> TeacherSubjects { get; set; }

        [ValidateNever]
        public virtual ApplicationUser User { get; set; }
    }
}
