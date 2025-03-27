using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Interface
{
    public interface IPublishEmailNotificationService
    {
        void SchedulePublishJobs(QuestionType questionType);
        void RemoveExistingJobs(int questionTypeId);
    }
}
