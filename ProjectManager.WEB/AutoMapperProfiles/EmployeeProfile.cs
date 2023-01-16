using AutoMapper;
using ProjectManager.BLL.DTO;
using ProjectManager.Domain.Entities;
using ProjectManager.WEB.ViewModels.EntityViewModel;

namespace ProjectManager.WEB.AutoMapperProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<EmployeeDTO, EmployeeViewModel>().ReverseMap();
        }
    }
}
