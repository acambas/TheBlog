using AutoMapper;
using Domain.HTMLPage;
using Domain.Post;
using Domain.Tag;
using Infrastructure.Helpers;
using System.Linq;
using WebUi.Models;
using WebUi.Models.Blog;
using WebUi.Models.HTMLPage;
using WebUi.Models.RavenDB;

namespace WebUi.App_Start
{
    public class MapperConfig
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<Post, PostViewModel>();

            Mapper.CreateMap<PostViewModel, Post>();
            Mapper.CreateMap<CreatePostViewModel, Post>()
               .ForMember(m => m.Tags,
               x => x.MapFrom(s => s.Tags.Select(z => new TagViewModel() { Name = z, UrlSlug = URLHelper.ToFriendlyUrl(z) })));

            Mapper.CreateMap<EditPostViewModel, Post>()
               .ForMember(m => m.Tags,
               x => x.MapFrom(s => s.Tags.Select(z => new TagViewModel() { Name = z, UrlSlug = URLHelper.ToFriendlyUrl(z) })));

            Mapper.CreateMap<Post, EditPostViewModel>()
               .ForMember(m => m.Tags,
               x => x.MapFrom(s => s.Tags.Select(z => z.Name)));

            Mapper.CreateMap<Post, PostListItemViewModel>()
                .ForMember(m => m.Tags,
                x => x.MapFrom(s => s.Tags.Select(z => z.Name)));

            Mapper.CreateMap<TagViewModel, Tag>()
                .ForMember(m => m.UrlSlug, x => x.MapFrom(s => URLHelper.ToFriendlyUrl(s.Name)));
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