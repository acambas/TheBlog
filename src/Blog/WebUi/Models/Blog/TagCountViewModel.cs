﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUi.Models.Blog
{
    public class TagCountViewModel
    {
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public int Count { get; set; }
    }
}