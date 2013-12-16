using AutoMapper;
using Domain.Post;
using Domain.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUi.Models;
using WebUi.Models.Blog;
using WebUi.Models.RavenDB;

namespace WebUi.App_Start
{
    public class MapperConfig
    {

        public static void ConfigureMappings()
        {
            Mapper.CreateMap<Post, PostViewModel>();
            Mapper.CreateMap<PostViewModel, Post>();

            Mapper.CreateMap<TagViewModel, Tag>();
            Mapper.CreateMap<Tag, TagViewModel>();

            Mapper.CreateMap<TagCountIndex.ReduceResult, TagCountViewModel>();
            Mapper.CreateMap<Post, PostLinkViewModel>();

            Mapper.CreateMap<ApplicationUser, ApplicationUserViewModel>();
            Mapper.CreateMap<ApplicationUserViewModel, ApplicationUser>();

            Mapper.CreateMap<ApplicationUser, RegisterViewModel>();
            Mapper.CreateMap<RegisterViewModel, ApplicationUser>();
        }

    }
}