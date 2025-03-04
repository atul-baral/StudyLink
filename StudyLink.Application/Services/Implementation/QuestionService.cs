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

        public async Task AddQuestionAsync(AddQuestionVM addQuestionVm)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            int teacherId = (int)await _teacherService.GetTeacherIdByUserIdAsync(user.Id);

            string subjectId = _httpContextAccessor.HttpContext.Session.GetString("SubjectId");
            string questionTypeId = _httpContextAccessor.HttpContext.Session.GetString("QuestionTypeId");

            var question = _mapper.Map<Question>(addQuestionVm);
            question.TeacherId = teacherId;
            question.SubjectId = int.Parse(subjectId);
            question.QuestionTypeId = int.Parse(questionTypeId);

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetAsync(u => u.QuestionId == id);
            if (question != null)
            {
                await _unitOfWork.Questions.DeleteAsync(question);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync(int id)
        {
            if (id > 0)
            {
                _httpContextAccessor.HttpContext.Session.SetString("QuestionTypeId", id.ToString());
            }
            int subjectId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("SubjectId"));
            int questionTypeId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("QuestionTypeId"));
            return await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId, includeProperties:"Choices");
        }

        public async Task<IEnumerable<AddAnswerVM>> GetAllQuestionsForAnswerAsync(int id)
        {
            if (id > 0)
            {
                _httpContextAccessor.HttpContext.Session.SetString("QuestionTypeId", id.ToString());
            }

            int subjectId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("SubjectId"));
            int questionTypeId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("QuestionTypeId"));

            var questions = await _unitOfWork.Questions.GetAllAsync(q => q.SubjectId == subjectId && q.QuestionTypeId == questionTypeId, includeProperties: "Choices");
            var mappedQuestions = _mapper.Map<List<AddAnswerVM>>(questions);
            return mappedQuestions;
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _unitOfWork.Questions.GetAsync(u => u.QuestionId == id, includeProperties: "Choices");
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            await _unitOfWork.Questions.UpdateAsync(question);
            await _unitOfWork.CompleteAsync();
        }
    }
}
