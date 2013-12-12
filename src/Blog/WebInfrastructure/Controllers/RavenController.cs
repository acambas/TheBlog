using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebInfrastructure.Controllers
{
    public class RavenController:Controller
    {
        public IDocumentSession RavenSession { get; protected set; }

        public RavenController()
        {

        }



    }
}
