using StudyLink.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace StudyLink.Application.ViewModels
{
    public class AddTeacherVM:Teacher
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
