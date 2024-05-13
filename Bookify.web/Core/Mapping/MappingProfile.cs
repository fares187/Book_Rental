using AutoMapper;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Microsoft.AspNetCore.DataProtection;
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
                .ForMember(dest => dest.BookCopies, opt => opt.MapFrom(p => p.copies))
                .ForMember(dest => dest.categories, opt => opt.MapFrom(p => p.categories.Select(c=>c.Category.Name).ToList()))
                ;


            CreateMap<BookCopy, BookCopyViewModel>()
               .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(p=>p.Book!.Title))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(p=>p.Book!.Id))
               .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(p=>p.Book!.ImageThumbnailUrl));
            CreateMap<BookCopyFormViewModel,BookCopy >()
                .ForMember(dest=>dest.BookId,opt=>opt.Ignore());

            //users
            CreateMap<ApplicationUser, UserViewModel>();


            //Governorates
            CreateMap<Governorate, SelectListItem>()
             .ForMember(dest => dest.Value, opt => opt.MapFrom(d => d.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(d => d.Name));

            //Areas
            CreateMap<Area, SelectListItem>()
             .ForMember(dest => dest.Value, opt => opt.MapFrom(d => d.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(d => d.Name));
          //    .ForPath(dest => dest.Group.Name, opt => opt.MapFrom(d => d.Governorate.Name));


            //subscripers
            CreateMap<Subscriper, SubscriperFormViewModel>().ReverseMap();
            CreateMap<Subscriper, SubscriperViewModel>()
                .ForMember(d => d.fullName, opt => opt.MapFrom(d => d.FirstName +" "+ d.LastName))
                .ForMember(d => d.ImageThumbnailUrl, opt => opt.MapFrom(d => d.ThumbnailUrl))
                
                ;
            CreateMap<Subscriper, SubscriberSearchViewModel>()
              .ForMember(d => d.FullName, opt => opt.MapFrom(d => d.FirstName + " " + d.LastName))
              .ForMember(d => d.thumbnailImageURL, opt => opt.MapFrom(d => d.ThumbnailUrl));
            CreateMap<Subscriper, SubscriperDetailsViewModel>()
                .ForMember(d => d.fullname, opt => opt.MapFrom(b => $"{b.FirstName} {b.LastName}"))
                .ForMember(d => d.Area, opt => opt.MapFrom(b => b.Area!.Name))
                .ForMember(d => d.Governorate, opt => opt.MapFrom(b => b.Governorate!.Name))
                ;


            CreateMap<Subscription, SubscriptionViewModel>();

            //rental
            CreateMap<Rental,RentalViewModel>();
            CreateMap<RentalCopy,RentalCopyViewModel>();
        }
    }
}
