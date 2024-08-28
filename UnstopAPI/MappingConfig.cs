using AutoMapper;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;

namespace UnstopAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Job, JobDTO>().ReverseMap();
            CreateMap<Candidate, CandidateDTO>().ReverseMap();
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<Application, ApplicationDTO>().ReverseMap();
            CreateMap<Interview, InterviewDTO>().ReverseMap();
            CreateMap<FavoriteJob, FavoriteDTO>().ReverseMap();
            CreateMap<UserPreference, UserPreferenceDTO>().ReverseMap();
            CreateMap<JobFair, JobFairDTO>().ReverseMap();
            CreateMap<Template, TemplateDTO>().ReverseMap();
            CreateMap<Element, ElementDTO>().ReverseMap();
        }
    }
}
