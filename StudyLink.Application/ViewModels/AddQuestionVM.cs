using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using StudyLink.Domain.Entities;
using System.Text.Json.Serialization;

namespace StudyLink.Application.ViewModels
{
    public class AddQuestionVM
    {
        [Key]
        public int QuestionId { get; set; }

        public int? SubjectId { get; set; }

        public int? TeacherId { get; set; }

        public int? QuestionTypeId { get; set; }

        public string QuestionText { get; set; }

        public int Marks { get; set; }

        //[JsonIgnore]
        public List<Choice> Choices { get; set; }

        [ValidateNever]
        public List<QuestionType> QuestionTypes { get; set; }
    }
}
