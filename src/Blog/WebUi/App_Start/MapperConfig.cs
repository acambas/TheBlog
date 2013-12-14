using AutoMapper;
using Domain.Post;
using Domain.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUi.Models.Blog;

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
        }

    }
}