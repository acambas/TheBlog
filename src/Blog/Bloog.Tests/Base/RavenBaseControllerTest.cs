using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebUi;
using WebUi.App_Start;
using WebUi.Models.RavenDB;

namespace Bloog.Tests.Base
{
    public class RavenBaseControllerTest
    {
        protected IAsyncDocumentSession RavenSession
        {
            get;
            set;
        }

        protected void SetUpIdentity()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );

            // User is logged in
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity("username"),
                new string[0]
                );
        }

        protected void SetUp()
        {
            MapperConfig.ConfigureMappings();
            MvcApplication.Store = new EmbeddableDocumentStore { RunInMemory = true };

            MvcApplication.Store.Initialize();
            RavenDbIndexes.SetUpIndexes(MvcApplication.Store);
            SetUpIdentity();
            
            RavenSession = MvcApplication.Store.OpenAsyncSession();
        }

        protected void CleanUp()
        {
            MvcApplication.Store.Dispose();
        }
    }
}
