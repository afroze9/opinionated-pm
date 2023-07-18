using AutoMapper;
using Nexus.PeopleAPI.DTO;
using Nexus.PeopleAPI.Entities;
using Nexus.PeopleAPI.Model;

namespace Nexus.PeopleAPI.Mapping;

[ExcludeFromCodeCoverage]
public class PeopleProfile : Profile
{
    public PeopleProfile()
    {
        CreateMap<Person, PersonDto>();
        CreateMap<PersonDto, Person>();
        
        CreateMap<PersonCreateRequestModel, Person>();
        CreateMap<PersonUpdateRequestModel, Person>();
        
        CreateMap<Person, PersonResponseModel>();
        CreateMap<PersonDto, PersonResponseModel>();
    }
}
