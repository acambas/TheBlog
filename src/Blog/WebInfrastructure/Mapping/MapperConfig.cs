﻿using AutoMapper;
using Domain.Post;
using Domain.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebInfrastructure.ViewModels;

namespace WebInfrastructure.Mapping
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
