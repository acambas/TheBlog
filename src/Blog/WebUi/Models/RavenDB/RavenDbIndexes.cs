using Raven.Client.Indexes;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Document;
using Domain.Post;
using Raven.Abstractions.Indexing;
using WebUi.Models.Blog;
using Domain.Tag;
using System.Linq.Expressions;

namespace WebUi.Models.RavenDB
{
    public class RavenDbIndexes
    {

        public static void SetUpIndexes(DocumentStore documentStore)
        {
            IndexCreation.CreateIndexes(typeof(TagCountIndex).Assembly, documentStore);
        }
    }


    public class PostsByTitleIndex : AbstractIndexCreationTask<Post>
    {
        public PostsByTitleIndex()
        {
            Map = posts => from post in posts
                           select new { post.Title };
            Index(x => x.Title, FieldIndexing.Analyzed);
            
        }
    }

    public class TagCountIndex : AbstractIndexCreationTask<Post, TagCountIndex.ReduceResult>
    {
        public class ReduceResult
        {
            public string Name { get; set; }
            public string UrlSlug { get; set; }
            public int Count { get; set; }
        }


        public TagCountIndex()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new
                           {
                               Name = tag.Name,
                               UrlSlug = tag.UrlSlug,
                               Count = 1
                           };

            Reduce = results => from result in results
                                group result by result.Name
                                into g 
                                select new 
                                {
                                    Name = g.Key,
                                    Count = g.Sum(m => m.Count),
                                    UrlSlug = g.First().UrlSlug
                                };
        }
    }
    
}