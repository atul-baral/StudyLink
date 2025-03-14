using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Services.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeacherService _teacherService;

        public QuestionService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ITeacherService teacherService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
            _teacherService = teacherService;
        }

        public async Task Add(AddQuestionVM question)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int teacherId = (int)await _teacherService.GetIdByUserId(user.Id);

            string subjectId = _httpContextAccessor.HttpContext.Session.GetString("SubjectId");
            string questionTypeId = _httpContextAccessor.HttpContext.Session.GetString("QuestionTypeId");

            var newQuestion = _mapper.Map<Question>(question);
            newQuestion.TeacherId = teacherId;
            newQuestion.SubjectId = int.Parse(subjectId);
            newQuestion.QuestionTypeId = int.Parse(questionTypeId);

            await _unitOfWork.Questions.AddAsync(newQuestion);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id)
        {
            var question = await _unitOfWork.Questions.GetAsync(u => u.QuestionId == id);
            if (question != null)
            {
                await _unitOfWork.Questions.DeleteAsync(question);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Question>> GetList(int subjectId)
        {
            return await _unitOfWork.Questions.GetAllAsync(
                q => q.SubjectId == subjectId,
                includeProperties: "Choices"
            );
        }

        public async Task<IEnumerable<AddAnswerVM>> GetListForAnswer(int subjectId)
        {
            var questions = await _unitOfWork.Questions.GetAllAsync(
                q => q.SubjectId == subjectId,
                includeProperties: "Choices"
            );
            return _mapper.Map<List<AddAnswerVM>>(questions);
        }

        public async Task<Question> GetById(int id)
        {
            return await _unitOfWork.Questions.GetAsync( u => u.QuestionId == id,  includeProperties: "Choices");
        }

        public async Task Update(Question question)
        {
            await _unitOfWork.Questions.UpdateAsync(question);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<int>> GetQuestionTypeList(int subjectId)
        {
            return (await _unitOfWork.Questions
                .GetAllAsync(q => q.SubjectId == subjectId && !q.IsDeleted))
                .Select(q => q.QuestionTypeId)
                .Distinct();
        }

        public async Task<int> GetQuestionCount(int subjectId, int questionTypeId)
        {
            return (await _unitOfWork.Questions
                .GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId))
                .Count();
        }
    }
}
