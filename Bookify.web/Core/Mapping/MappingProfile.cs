using AutoMapper;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Categories
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Category, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(d => d.Id))
                 .ForMember(dest => dest.Text, opt => opt.MapFrom(d => d.Name));



            CreateMap<CreateCategoriesViewModel, Category>().ReverseMap();

            //Authors
            CreateMap<AuthorForm, Author>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(d => d.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(d => d.Name));

            //Books
            CreateMap<BookFormViewModel, Book>().ReverseMap()
                .ForMember(dest=>dest.Categories,opt=>opt.Ignore());
            CreateMap<BookViewModel, Book>().ReverseMap()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(p=> p.Author.Name))
                .ForMember(dest => dest.categories, opt => opt.MapFrom(p => p.categories.Select(c=>c.Category.Name).ToList()))
                ;

        }
    }
}
