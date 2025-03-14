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
        private readonly ISubjectService _subjectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentSubjectsViewComponent(ISubjectService subjectService, UserManager<ApplicationUser> userManager)
        {
            _subjectService = subjectService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var studentSubjects = await _subjectService.GetListByUserId(user.Id);

            var subjects = new List<Subject>();

            foreach (var studentSubject in studentSubjects)
            {
                subjects.Add(new Subject
                {
                    SubjectId = studentSubject.SubjectId,
                    SubjectName = studentSubject.SubjectName,
                    SubjectCode = studentSubject.SubjectCode
                });
            }

            return View(subjects);

        }
    }
}
