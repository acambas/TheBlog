using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebUi;

namespace Bloog.Tests.Base
{
    public class RavenBaseController
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
            MvcApplication.Store = new EmbeddableDocumentStore { RunInMemory = true };
            MvcApplication.Store.Initialize();

            SetUpIdentity();

            RavenSession = MvcApplication.Store.OpenAsyncSession();
        }

        protected void CleanUp()
        {
            MvcApplication.Store.Dispose();
        }
    }
}
