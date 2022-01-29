using AutoMapper;
using BookStoreAPI.Data;
using BookStoreAPI.DTOs.Author;

namespace BookStoreAPI.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            //Reverse allows to use map in both directions
            CreateMap<AuthorCreateDto, Author>().ReverseMap();
            CreateMap<AuthorUpdateDto, Author>().ReverseMap();
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap();
        }
    }
}
