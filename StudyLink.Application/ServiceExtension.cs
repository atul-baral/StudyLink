using Microsoft.Extensions.DependencyInjection;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Implementation;
using StudyLink.Application.Services.Interface;

namespace StudyLink.Application
{
    public static class ServiceExtension
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IQuestionTypeService, QuestionTypeService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPublishEmailNotificationService, PublishEmailNotificationService>();
        }
    }
}
