using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Identity;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Implementation
{
     internal class PublishEmailNotificationService : IPublishEmailNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PublishEmailNotificationService(IUnitOfWork unitOfWork, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
        }

        public void SchedulePublishJobs(QuestionType questionType)
        {
            if (questionType.PublishDate != null)
            {
                DateTime publishDate = questionType.PublishDate;
                BackgroundJob.Schedule(() => NotifyTeachersForUnpublishedSubjects(questionType.QuestionTypeId), publishDate.AddMinutes(-2));
                BackgroundJob.Schedule(() => NotifyAdminOnPublishStatus(questionType.QuestionTypeId), publishDate.AddMinutes(-1));
                BackgroundJob.Schedule(() => NotifyStudentsAboutPublishedSubjects(questionType.QuestionTypeId), publishDate.AddMinutes(-1));
                BackgroundJob.Schedule(() => ExecutePublishJob(questionType.QuestionTypeId), publishDate.AddSeconds(-30));
            }
        }

        public void RemoveExistingJobs(int questionTypeId)
        {
            BackgroundJob.Delete($"email_job_1_{questionTypeId}");
            BackgroundJob.Delete($"email_job_2_{questionTypeId}");
            BackgroundJob.Delete($"email_job_3_{questionTypeId}");
            BackgroundJob.Delete($"execute_job_{questionTypeId}");
        }

        public async Task NotifyTeachersForUnpublishedSubjects(int questionTypeId)
        {
            var unpublishedSubjects = await _unitOfWork.Subjects
                .GetAllAsync(s => s.SubjectQuestionTypes.Any(sqt => sqt.QuestionTypeId == questionTypeId && !sqt.IsPublished),
                             includeProperties: "TeacherSubjects");

            foreach (var subject in unpublishedSubjects)
            {
                foreach (var teacherSubject in subject.TeacherSubjects)
                {
                    var teacher = teacherSubject.Teacher;
                    string subjectLine = $"Subject Not Published: {subject.SubjectName}";
                    string body = $"Dear {teacher.User.UserName}, your subject '{subject.SubjectName}' has not been published yet. Please review.";
                    await _emailService.SendEmailAsync(teacher.User.Email, subjectLine, body);
                }
            }
        }

        public async Task NotifyAdminOnPublishStatus(int questionTypeId)
        {
            var subjects = await _unitOfWork.Subjects
                .GetAllAsync(s => s.SubjectQuestionTypes.Any(sqt => sqt.QuestionTypeId == questionTypeId),
                             includeProperties: "SubjectQuestionTypes");

            var publishedSubjects = subjects.Where(s => s.SubjectQuestionTypes.Any(sqt => sqt.IsPublished)).ToList();
            var unpublishedSubjects = subjects.Where(s => s.SubjectQuestionTypes.Any(sqt => !sqt.IsPublished)).ToList();

            var adminEmails = await GetAdminEmail();

            string subject = "Subject Publish Status";
            string body = $"Published Subjects: {string.Join(", ", publishedSubjects.Select(s => s.SubjectName))}\n" +
                          $"Unpublished Subjects: {string.Join(", ", unpublishedSubjects.Select(s => s.SubjectName))}";

            foreach (var adminEmail in adminEmails)
            {
                await _emailService.SendEmailAsync(adminEmail, subject, body);
            }
        }

        public async Task NotifyStudentsAboutPublishedSubjects(int questionTypeId)
        {
            var publishedSubjects = await _unitOfWork.Subjects
                .GetAllAsync(s => s.SubjectQuestionTypes.Any(sqt => sqt.QuestionTypeId == questionTypeId && sqt.IsPublished),
                             includeProperties: "StudentSubjects");

            foreach (var subject in publishedSubjects)
            {
                foreach (var studentSubject in subject.StudentSubjects)
                {
                    var student = studentSubject.Student;
                    string subjectLine = $"New Published Subject: {subject.SubjectName}";
                    string body = $"Dear {student.User.UserName}, the subject '{subject.SubjectName}' has been published. You can access it now.";
                    await _emailService.SendEmailAsync(student.User.Email, subjectLine, body);
                }
            }
        }

        public async Task NotifyAdminAfterExecution(int questionTypeId)
        {
            var failedSubjects = await _unitOfWork.Subjects
                .GetAllAsync(s => s.SubjectQuestionTypes.Any(sqt => sqt.QuestionTypeId == questionTypeId && !sqt.IsPublished));

            var adminEmails = await GetAdminEmail();

            string subject = "Unpublished Subjects After Execution";
            string body = $"Dear Admin, the following subjects remain unpublished: {string.Join(", ", failedSubjects.Select(s => s.SubjectName))}.";

            foreach (var adminEmail in adminEmails)
            {
                await _emailService.SendEmailAsync(adminEmail, subject, body);
            }
        }

        public async Task NotifyTeachersAfterExecution(int questionTypeId)
        {
            await NotifyTeachersForUnpublishedSubjects(questionTypeId);
        }

        public async Task ExecutePublishJob(int questionTypeId)
        {
            var subjectQuestionTypes = await _unitOfWork.SubjectQuestionTypes
                .GetAllAsync(sqt => sqt.QuestionTypeId == questionTypeId, includeProperties: "Subject");

            foreach (var subjectQuestionType in subjectQuestionTypes)
            {
                    var currentStatus = await _unitOfWork.SubjectQuestionTypes
                        .GetAsync(sqt => sqt.QuestionTypeId == questionTypeId && sqt.SubjectId == subjectQuestionType.SubjectId);

                    if (currentStatus != null && currentStatus.IsPublished == false)
                    {
                        currentStatus.IsPublished = true;
                        await _unitOfWork.SubjectQuestionTypes.UpdateAsync(currentStatus);
                    }
               
            }
            await Task.WhenAll(NotifyAdminAfterExecution(questionTypeId), NotifyTeachersAfterExecution(questionTypeId));
        }


        private async Task<List<string>> GetAdminEmail()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminEmails = adminUsers.Select(u => u.Email).ToList();
            return adminEmails;
        }
    }
}
