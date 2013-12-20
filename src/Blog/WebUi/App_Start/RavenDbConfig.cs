using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Domain.Post;
using Domain.Tag;
using Microsoft.AspNet.Identity;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using WebUi.Models;
using WebUi.Models.RavenDB;

namespace WebUi.App_Start
{
    public class RavenDbConfig
    {
        private static void seedData()
        {
            string description = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc faucibus diam ut malesuada mattis. Praesent ultricies, nunc vel semper gravida, tellus leo consectetur orci, eget accumsan elit ligula non mi. Integer et tellus eget ante lacinia dictum. Sed quis semper massa. Donec varius ultricies felis, volutpat commodo neque iaculis nec. Curabitur at lacus porttitor, tincidunt dui vitae, ullamcorper sem. Nulla elit mi, tincidunt id orci eget, mattis pulvinar orci. Ut mattis placerat condimentum. Mauris neque enim, mollis at justo ac, tristique aliquet mauris. Etiam lacinia venenatis ullamcorper. Sed a elit non ligula pellentesque faucibus. Integer et massa mauris. Sed luctus venenatis tortor, sed imperdiet urna consectetur id. Suspendisse sed elementum massa, id feugiat tortor. Pellentesque gravida, nibh eu imperdiet ornare, est arcu egestas dolor, eget sagittis metus lorem eget arcu. Curabitur id scelerisque orci.</p>";
            string shortDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc faucibus diam ut malesuada mattis. Praesent ultricies, nunc vel semper gravida, tellus leo consectetur orci, eget accumsan elit ligula non mi. Integer et tellus eget ante lacinia dictum. Sed quis semper massa. Donec varius ultricies felis, volutpat commodo neque iaculis nec. Curabitur at lacus porttitor, tincidunt dui vitae, ullamcorper sem. Nulla elit mi, tincidunt id orci eget, mattis pulvinar orci. Ut mattis placerat condimentum. Mauris neque enim, mollis at justo ac, tristique aliquet mauris. Etiam lacinia venenatis ullamcorpe";

            Tag t1 = new Tag { Name = ".NET", UrlSlug = "dotNET" };
            Tag t2 = new Tag { Name = "ASP.NET MVC", UrlSlug = "ASPdotNET_MVC" };
            Tag t3 = new Tag { Name = "Web", UrlSlug = "Web" };
            Tag t4 = new Tag { Name = "Db", UrlSlug = "Db" };

            Tag[] tagsArray = new Tag[] { t1, t2, t3, t4 };

            using (var session = MvcApplication.Store.OpenSession())
            {
                Random r = new Random();
                int postNumber = r.Next(3, 7);
                for (int i = 0; i < postNumber; i++)
                {
                    var post = new Post()
                    {
                        Title = "Post " + (i + 1),
                        ShortDescription = shortDescription,
                        Description = description,
                        UrlSlug = "Post-" + (i + 1) + "__" + (i + 1) + "__",
                        LastEdit = DateTime.Now,
                        PostedOn = DateTime.Now,
                        Published = true,
                        Tags = new List<Tag>()
                    };

                    int numOfTagForPost = r.Next(1, tagsArray.Length);

                    for (int j = 0; j < numOfTagForPost; j++)
                    {
                        post.Tags = tagsArray.Take(r.Next(1, tagsArray.Length)).ToList();
                        //post.Tags.Add(tagsArray[r.Next(tagsArray.Length)]);
                    }

                    session.Store(post);
                }

                session.SaveChanges();
            }
            UserInit();
        }

        private static void UserInit()
        {
            using (var session = MvcApplication.Store.OpenAsyncSession())
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new RavenUserStore<ApplicationUser>(MvcApplication.Store.OpenAsyncSession()));

                var userAdmin = UserManager.FindByName("Admin");
                if (userAdmin == null)
                {
                    ApplicationUser addUser = new ApplicationUser()
                    {
                        UserName = "Admin",
                        Claims = new List<RavenUserClaim>
                        {
                            new RavenUserClaim(new Claim(ClaimTypes.Role, AppRoles.Admin))
                        }
                    };
                    var userAfterCreate = UserManager.CreateAsync(addUser, ConfigurationManager.AppSettings["AdminPassword"]).Result;
                }

                var userEditor = UserManager.FindByName("Editor");
                if (userEditor == null)
                {
                    ApplicationUser addUser = new ApplicationUser()
                    {
                        UserName = "Editor",
                        Claims = new List<RavenUserClaim>
                        {
                            new RavenUserClaim(new Claim(ClaimTypes.Role, AppRoles.Edit))
                        }
                    };
                    var userAfterCreate = UserManager.CreateAsync(addUser, ConfigurationManager.AppSettings["AdminPassword"]).Result;
                }

                var userReader = UserManager.FindByName("Reader");
                if (userReader == null)
                {
                    ApplicationUser addUser = new ApplicationUser()
                    {
                        UserName = "Reader",
                        Claims = new List<RavenUserClaim>
                        {
                            new RavenUserClaim(new Claim(ClaimTypes.Role, AppRoles.Read))
                        }
                    };
                    var userAfterCreate = UserManager.CreateAsync(addUser, ConfigurationManager.AppSettings["AdminPassword"]).Result;
                }
            }
        }

        public static void RavenDBInit()
        {
#if DEBUG
            //Store = new EmbeddableDocumentStore {  ConnectionStringName = "RavenDBEmbedded" };

            MvcApplication.Store = new EmbeddableDocumentStore() { RunInMemory = true };
            MvcApplication.Store.Initialize();
            RavenDbIndexes.SetUpIndexes(MvcApplication.Store);
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), MvcApplication.Store);
            seedData();

#else
            MvcApplication.Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            MvcApplication.Store.Initialize();
            RavenDbIndexes.SetUpIndexes(MvcApplication.Store);
            //IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), MvcApplication.Store);
#endif
        }
    }

    public class RavenLogMessage
    {
        public RavenLogMessage()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }

    public class RavenLog:Infrastructure.Logging.ILogger
    {
        DocumentStore store = MvcApplication.Store;
        public void Log(string message)
        {
            using (var session = store.OpenSession())
            {
                RavenLogMessage log = new RavenLogMessage() { Message = message };
                session.Store(log);
                session.SaveChanges();
            }
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception e)
        {
            throw new NotImplementedException();
        }
    }
}