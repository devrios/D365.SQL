namespace D365.SQL.D365
{
    using System;
    using Microsoft.Xrm.Sdk.Client;

    internal class CRMInstance
    {
        public CRMInstance(OrganizationServiceProxy service)
        {
            Service = service;
        }

        public OrganizationServiceProxy Service { get; }

        public string Name => CRMRegion(Service.EndpointSwitch.PrimaryEndpoint);

        public string Region => CRMInstanceName(Service.EndpointSwitch.PrimaryEndpoint);

        private string CRMInstanceName(Uri uri)
        {
            var uriString = uri.ToString();
            var startText = uriString.StartsWith("https", StringComparison.OrdinalIgnoreCase)
                ? "https://"
                : "http://";
            var endText = ".api.";

            var startIndex = uriString.IndexOf(startText, StringComparison.OrdinalIgnoreCase);

            if (startIndex < 0)
            {
                throw new Exception($"Unable to find region in url '{uriString}'");
            }

            var endIndex = uriString.IndexOf(endText, startIndex + 1, StringComparison.Ordinal);

            if (endIndex < 0)
            {
                throw new Exception($"Unable to find region in url '{uriString}'");
            }

            var instanceName = uriString.Substring((startIndex = startIndex + startText.Length), endIndex - startIndex);

            return instanceName;
        }

        private string CRMRegion(Uri uri)
        {
            var findText = ".api.";
            var uriString = uri.ToString();
            var startIndex = uriString.IndexOf(findText, StringComparison.OrdinalIgnoreCase);

            if (startIndex < 0)
            {
                throw new Exception($"Unable to find region in url '{uriString}'");
            }

            var endIndex = uriString.IndexOf(".", startIndex + 1, StringComparison.Ordinal);

            if (endIndex < 0)
            {
                throw new Exception($"Unable to find region in url '{uriString}'");
            }

            var region = uriString.Substring(startIndex + findText.Length, endIndex - startIndex);

            return region;
        }
    }
}