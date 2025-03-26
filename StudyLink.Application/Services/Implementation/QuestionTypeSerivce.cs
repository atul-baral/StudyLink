using Hangfire;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyLink.Application.Services.Implementation
{
    internal class QuestionTypeService : IQuestionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<QuestionType>> GetList()
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync(includeProperties: "SubjectQuestionTypes");
            return questionTypes.OrderBy(q => q.SortOrder);
        }
        
        public async Task<IEnumerable<QuestionType>> GetListBySubjectId(int subjectId)
        {
            var subjectQuestionTypes = await _unitOfWork.SubjectQuestionTypes
                .GetAllAsync(sqt => sqt.SubjectId == subjectId && !sqt.IsDeleted, includeProperties: "QuestionType");

            var questionTypes = subjectQuestionTypes
                .Select(sqt => sqt.QuestionType)
                .Distinct()
                .Where(qt => !qt.IsDeleted)
                .OrderByDescending(qt => qt.SortOrder)
                .ToList();
            return questionTypes;
        }

        public async Task<QuestionType> GetById(int id)
        {
            return await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == id);
        }

        public async Task Add(QuestionType questionType)
        {
            var questionTypes = await _unitOfWork.QuestionTypes.GetAllAsync();
            var maxSortOrder = questionTypes.OrderByDescending(q => q.SortOrder).FirstOrDefault();
            questionType.SortOrder = maxSortOrder != null ? maxSortOrder.SortOrder + 1 : 1;

            var subjects = await _unitOfWork.Subjects.GetAllAsync();

            questionType.SubjectQuestionTypes = subjects.Select(subject => new SubjectQuestionType
            {
                SubjectId = subject.SubjectId, 
                QuestionTypeId = questionType.QuestionTypeId 
            }).ToList();

            await _unitOfWork.QuestionTypes.AddAsync(questionType);
            await _unitOfWork.CompleteAsync();

            SchedulePublishJobs(questionType);
        }

        public async Task Update(QuestionType questionType)
        {
            await _unitOfWork.QuestionTypes.UpdateAsync(questionType);
            await _unitOfWork.CompleteAsync();

            RemoveExistingJobs(questionType.QuestionTypeId);
            SchedulePublishJobs(questionType);
        }

        public async Task Delete(int id)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == id);
            if (questionType != null)
            {
                await _unitOfWork.QuestionTypes.DeleteAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task UpdateOrder(List<QuestionType> updatedQuestionTypes)
        {
            foreach (var questionType in updatedQuestionTypes)
            {
                var existingQuestionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionType.QuestionTypeId);
                if (existingQuestionType != null)
                {
                    existingQuestionType.SortOrder = questionType.SortOrder;
                    await _unitOfWork.QuestionTypes.UpdateAsync(existingQuestionType);
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<QuestionType>> GetPublishedListBySubjectId(int subjectId)
        {
            var subjectQuestionTypes = await _unitOfWork.SubjectQuestionTypes
                .GetAllAsync(sqt => sqt.SubjectId == subjectId && sqt.IsPublished && !sqt.IsDeleted, includeProperties: "QuestionType");

            var questionTypes = subjectQuestionTypes
                .Select(sqt => sqt.QuestionType)
                .Distinct()
                .Where(qt => !qt.IsDeleted) 
                .OrderByDescending(qt => qt.SortOrder)
                .ToList();
            return questionTypes;
        }

        public async Task<int> GetQuestionMarksDifferenceFromFullMarks(int questionTypeId, int subjectId)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionTypeId);
            var totalQuestionMarks = (await _unitOfWork.Questions.GetAllAsync(q => q.QuestionTypeId == questionTypeId && q.SubjectId == subjectId && !q.IsDeleted)).Sum(q => q.Marks);
            return questionType.FullMarks - totalQuestionMarks; 
        }

        public async Task<int> TogglePublishStatus(int questionTypeId,int subjectId, bool isPublished)
        {
            if (await GetQuestionMarksDifferenceFromFullMarks(questionTypeId, subjectId) != 0)
            {
                return 0;
            }

            var questionType = await _unitOfWork.SubjectQuestionTypes.GetAsync(q => q.QuestionTypeId == questionTypeId && q.SubjectId == subjectId);
            if (questionType != null)
            {
                questionType.IsPublished = isPublished;
                await _unitOfWork.SubjectQuestionTypes.UpdateAsync(questionType);
                await _unitOfWork.CompleteAsync();
            }
            return 1;
        }

        public async Task<IEnumerable<SubjectQuestionType>> GetQuestionTypeDetails(int questionTypeId)
        {
           var subjects = await _unitOfWork.SubjectQuestionTypes.GetAllAsync(x=> x.QuestionTypeId == questionTypeId, includeProperties: "Subject");
            return subjects;
        }

        private void SchedulePublishJobs(QuestionType questionType)
        {
            if (questionType.PublishDate != null)
            {
                DateTime publishDate = questionType.PublishDate;

                BackgroundJob.Schedule(() => SendEmailNotification(questionType.QuestionTypeId), publishDate.AddMinutes(-2)); 
                BackgroundJob.Schedule(() => SendEmailNotification(questionType.QuestionTypeId), publishDate.AddMinutes(-1));  
                BackgroundJob.Schedule(() => ExecutePublishJob(questionType.QuestionTypeId), publishDate.AddMinutes(-5));    
            }
        }

        private void RemoveExistingJobs(int questionTypeId)
        {
            BackgroundJob.Delete($"email_job_1_{questionTypeId}");
            BackgroundJob.Delete($"email_job_2_{questionTypeId}");
            BackgroundJob.Delete($"publish_job_{questionTypeId}");
        }

        public async Task ExecutePublishJob(int questionTypeId)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionTypeId);
            var subjectQuestionTypes = await _unitOfWork.SubjectQuestionTypes
                .GetAllAsync(sqt => sqt.QuestionTypeId == questionTypeId, includeProperties: "Subject");

            List<string> failedSubjects = new List<string>();

            foreach (var subjectQuestionType in subjectQuestionTypes)
            {
                try
                {
                    int result = await TogglePublishStatus(questionTypeId, subjectQuestionType.SubjectId, true);
                    if (result == 1)
                    {
                        Console.WriteLine($"✅ Successfully published Question Type '{questionType.TypeName}' for Subject '{subjectQuestionType.Subject.SubjectName}'.");
                    }
                    else
                    {
                        failedSubjects.Add(subjectQuestionType.Subject.SubjectName);
                    }
                }
                catch (Exception ex)
                {
                    failedSubjects.Add(subjectQuestionType.Subject.SubjectName);
                    Console.WriteLine($"❌ Error publishing '{subjectQuestionType.Subject.SubjectName}': {ex.Message}");
                }
            }

            if (failedSubjects.Any())
            {
                Console.WriteLine($"⚠️ Failed to publish for the following subjects: {string.Join(", ", failedSubjects)}");
            }
        }

        public async Task SendEmailNotification(int questionTypeId)
        {
            var questionType = await _unitOfWork.QuestionTypes.GetAsync(q => q.QuestionTypeId == questionTypeId);
            if (questionType != null)
            {
                Console.WriteLine($"📧 Email notification sent for Question Type: {questionType.TypeName}");
            }
        }
    }
}
