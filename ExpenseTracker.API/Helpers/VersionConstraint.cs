using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.API.Helpers
{
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Web.Http.Routing;

    public class VersionConstraint : IHttpRouteConstraint
    {
        public int AllowedVersion { get; private set; }

        public const string VersionHeaderName = "api-version";

        private const int DefaultVersion = 1;

        public VersionConstraint(int allowedVersion)
        {
            this.AllowedVersion = allowedVersion;
        }

        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                int? version = this.GetVersionFromCustomRequestHeader(request);

                if (version == null)
                {
                    version = this.GetVersionFromCustomContentType(request);
                }

                return ((version ?? DefaultVersion) == AllowedVersion);
            }

            return true;
        }

        private int? GetVersionFromCustomContentType(HttpRequestMessage request)
        {
            var mediaTypes = request.Headers.Accept.Select(x => x.MediaType);

            var regEx = new Regex(@"application\/vnd\.expensetrackerapi\.v([\d]+)\+json");

            var matchingMediaType = mediaTypes.FirstOrDefault(regEx.IsMatch);

            if (matchingMediaType == null)
            {
                return null;
            }

            System.Text.RegularExpressions.Match m = regEx.Match(matchingMediaType);
            string versionAsString = m.Groups[1].Value;

            int version;
            if (Int32.TryParse(versionAsString, out version))
            {
                return version;
            }

            return null;
        }

        private int? GetVersionFromCustomRequestHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;

            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }

            int version;
            if (versionAsString != null && Int32.TryParse(versionAsString, out version))
            {
                return version;
            }

            return null;
        }
    }
}
