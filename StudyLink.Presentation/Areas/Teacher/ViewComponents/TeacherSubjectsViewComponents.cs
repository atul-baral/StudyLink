using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.ViewComponents
{
    public class TeacherSubjectsViewComponent : ViewComponent
    {
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherSubjectsViewComponent(ITeacherService teacherService, UserManager<ApplicationUser> userManager)
        {
            _teacherService = teacherService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var teacher = await _teacherService.GetTeacherByUserIdAsync(user.Id);
            if (teacher == null || teacher.TeacherSubjects == null) return Content("No subjects assigned.");

            var subjects = new List<Subject>();

            foreach (var teacherSubject in teacher.TeacherSubjects)
            {
                subjects.Add(new Subject
                {
                    SubjectId = teacherSubject.Subject.SubjectId,
                    SubjectName = teacherSubject.Subject.SubjectName,
                    SubjectCode = teacherSubject.Subject.SubjectCode
                });
            }

            return View(subjects);

        }
    }
}
