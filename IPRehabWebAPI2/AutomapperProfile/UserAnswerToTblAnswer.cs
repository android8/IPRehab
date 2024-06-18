using AutoMapper;
using IPRehabModel;
using IPRehabWebAPI2.Models;

namespace IPRehabWebAPI2.AutomapperProfile
{
    public class UserAnswerToTblAnswer : Profile
    {
        public UserAnswerToTblAnswer()
        {
            CreateMap<UserAnswer, tblAnswer>()
                .ForMember(dest => dest.EpsideOfCareIDFK, cfg => cfg.MapFrom(src => src.EpisodeID))
                .ForMember(dest => dest.QuestionIDFK, cfg => cfg.MapFrom(src => src.QuestionID))
                .ForMember(dest => dest.MeasureIDFK, cfg => cfg.MapFrom(src => src.MeasureID))
                .ForMember(dest => dest.AnswerCodeSetFK, cfg => cfg.MapFrom(src => src.AnswerCodeSetID))
                .ForMember(dest => dest.AnswerSequenceNumber, cfg => cfg.MapFrom(src => src.AnswerSequenceNumber))
                .ForMember(dest => dest.Description, cfg => cfg.MapFrom(src => src.Description))
                .ForMember(dest => dest.AnswerByUserID, cfg => cfg.MapFrom(src => src.AnswerByUserID))
                .ForMember(dest => dest.LastUpdate, cfg => cfg.MapFrom(src => src.LastUpdate))
                .ReverseMap();
        }
    }
}
