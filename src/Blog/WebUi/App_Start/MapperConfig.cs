using AutoMapper;
using Domain.Post;
using Domain.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using WebUi.Models;
using WebUi.Models.Blog;
using WebUi.Models.RavenDB;
using Infrastructure.Helpers;
using Domain.HTMLPage;
using WebUi.Models.HTMLPage;
namespace WebUi.App_Start
{
    public class MapperConfig
    {

        public static void ConfigureMappings()
        {
            Mapper.CreateMap<Post, PostViewModel>();
            Mapper.CreateMap<PostViewModel, Post>()
                .ForMember(m => m.Tags,
                x => x.MapFrom(s => s.Tags.Select(z => new TagViewModel() { Name = z, UrlSlug = URLHelper.ToFriendlyUrl(z) })));

            Mapper.CreateMap<Post, PostListItemViewModel>()
                .ForMember(m => m.Tags,
                x => x.MapFrom(s => s.Tags.Select(z => z.Name)));
            //PostListItemViewModel

            Mapper.CreateMap<TagViewModel, Tag>()
                .ForMember(m=>m.UrlSlug , x=>x.MapFrom(s => URLHelper.ToFriendlyUrl(s.Name)));
            Mapper.CreateMap<Tag, TagViewModel>();

            Mapper.CreateMap<TagCountIndex.ReduceResult, TagCountViewModel>();
            Mapper.CreateMap<Post, PostLinkViewModel>();

            Mapper.CreateMap<ApplicationUser, ApplicationUserViewModel>();
            Mapper.CreateMap<ApplicationUserViewModel, ApplicationUser>();

            Mapper.CreateMap<ApplicationUser, RegisterViewModel>();
            Mapper.CreateMap<RegisterViewModel, ApplicationUser>();

            Mapper.CreateMap<HTMLPage, HTMLPageViewModel>();
            Mapper.CreateMap<HTMLPageViewModel, HTMLPage>();
        }

    }
}