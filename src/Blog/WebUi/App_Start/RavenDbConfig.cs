using Domain.Post;
using Domain.Tag;
using Raven.Abstractions.Data;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebUi.App_Start
{
    public class RavenDbConfig
    {

        public static void RavenDBInit()
        {
#if DEBUG
            //Store = new EmbeddableDocumentStore {  ConnectionStringName = "RavenDBEmbedded" };

            MvcApplication.Store = new EmbeddableDocumentStore() { RunInMemory = true };
            MvcApplication.Store.Initialize();
            using (var session = MvcApplication.Store.OpenSession())
            {
                var post = new Post()
                {
                    Title = "Post 1",
                    ShortDescription = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf",
                    Description = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf asdasfsa asfasf asf asd asf asf safsa fasfs",
                    UrlSlug = "Post-1__1__",
                    LastEdit = DateTime.Now,
                    PostedOn = DateTime.Now,
                    Published = true,
                    Tags = new List<Tag> { new Tag { Name = "mvc", UrlSlug = "mvc" } }
                };

                session.Store(post);
                session.SaveChanges();
            }

#else
            Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            Store.Initialize();
#endif
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), MvcApplication.Store);
        }

    }
}