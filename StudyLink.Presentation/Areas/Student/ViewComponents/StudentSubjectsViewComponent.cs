using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.ViewComponents
{
    public class StudentSubjectsViewComponent : ViewComponent
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentSubjectsViewComponent(IStudentService studentService, UserManager<ApplicationUser> userManager)
        {
            _studentService = studentService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null || student.StudentSubjects == null) return Content("No subjects assigned.");

            var subjects = new List<Subject>();

            foreach (var studentSubject in student.StudentSubjects)
            {
                subjects.Add(new Subject
                {
                    SubjectId = studentSubject.Subject.SubjectId,
                    SubjectName = studentSubject.Subject.SubjectName,
                    SubjectCode = studentSubject.Subject.SubjectCode
                });
            }

            return View(subjects);

        }
    }
}
