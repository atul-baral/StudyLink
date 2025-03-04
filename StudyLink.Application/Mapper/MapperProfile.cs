using AutoMapper;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;

namespace StudyLink.Application.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AddStudentVM, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => !string.IsNullOrEmpty(src.UserId))) //because identity autogenerates Id while adding
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<AddTeacherVM, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => !string.IsNullOrEmpty(src.UserId))) //because identity autogenerates Id while adding
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<Student, AddStudentVM>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<Teacher, AddTeacherVM>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<Question, AddQuestionVM>()
                .ReverseMap();

            CreateMap<Question, AddAnswerVM>();

            CreateMap<Choice, ChoiceVM>()
                .ForMember(dest => dest.ChoiceId, opt => opt.MapFrom(src => src.ChoiceId))
                .ForMember(dest => dest.ChoiceText, opt => opt.MapFrom(src => src.ChoiceText));

            CreateMap<Answer, AddAnswerVM>()
                .ForPath(dest => dest.Answer.ChoiceId, opt => opt.MapFrom(src => src.SelectedChoiceId))
                .ReverseMap();


        }
    }
}
 