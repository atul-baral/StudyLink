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
        private readonly ISubjectService _subjectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherSubjectsViewComponent(ISubjectService subjectService, UserManager<ApplicationUser> userManager)
        {
            _subjectService = subjectService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var teacherSubjects = await _subjectService.GetListByUserId(user.Id);

            var subjects = new List<Subject>();

            foreach (var teacherSubject in teacherSubjects)
            {
                subjects.Add(new Subject
                {
                    SubjectId = teacherSubject.SubjectId,
                    SubjectName = teacherSubject.SubjectName,
                    SubjectCode = teacherSubject.SubjectCode
                });
            }

            return View(subjects);

        }
    }
}
