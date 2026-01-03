using AutoMapper;
using Employee.Application.DTOs;
using Employee.Domain.Aggregates;
using Employee.Domain.Entities;

namespace Employee.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EmployeeAggregate, EmployeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
            .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
            .ForMember(dest => dest.YearsOfService, opt => opt.MapFrom(src => src.GetYearsOfService()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto
            {
                Street = src.Address.Street,
                Number = src.Address.Number,
                Complement = src.Address.Complement,
                Neighborhood = src.Address.Neighborhood,
                City = src.Address.City,
                State = src.Address.State,
                ZipCode = src.Address.ZipCode,
                Country = src.Address.Country
            }));

        CreateMap<Department, DepartmentDto>();
    }
}
