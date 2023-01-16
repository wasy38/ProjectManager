using AutoMapper;
using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using ProjectManager.WEB.ViewModels.EntityViewModel;

namespace ProjectManager.WEB.AutoMapperProfiles
{
    public class ObjectiveProfile : Profile
    {
        public ObjectiveProfile()
        {
            CreateMap<Objective, ObjectiveDTO>().ReverseMap();
            CreateMap<ObjectiveDTO, ObjectiveViewModel>().ReverseMap();
        }
    }
}
