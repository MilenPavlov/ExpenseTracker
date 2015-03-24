using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.API.Helpers
{
    using System.Web.Http.Routing;

    public class VersionedRoute : RouteFactoryAttribute
    {
        public int AllowedVersion { get; private set; }

        public VersionedRoute(string template, int allowedVersion)
            : base(template)
        {
            this.AllowedVersion = allowedVersion;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", new VersionConstraint(AllowedVersion));
                return constraints;
            }
        }
    }
}
