using AutoMapper;
using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using ProjectManager.WEB.ViewModels.EntityViewModel;

namespace ProjectManager.WEB.AutoMapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDTO>().ReverseMap();
            CreateMap<ProjectDTO, ProjectViewModel>().ReverseMap();
        }
    }
}
