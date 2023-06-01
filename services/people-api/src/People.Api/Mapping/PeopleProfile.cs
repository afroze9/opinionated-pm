using AutoMapper;
using PeopleAPI.DTO;
using PeopleAPI.Entities;
using PeopleAPI.Model;

namespace PeopleAPI.Mapping;

[ExcludeFromCodeCoverage]
public class PeopleProfile : Profile
{
    public PeopleProfile()
    {
        CreateMap<Person, PersonDto>();

        CreateMap<PersonDto, Person>();
        CreateMap<PersonDto, PersonResponseModel>();
        
        CreateMap<PersonCreateRequestModel, PersonDto>();
        CreateMap<PersonUpdateRequestModel, PersonDto>();
    }
}
