namespace D365.SQL.D365
{
    using System;
    using System.ServiceModel.Description;
    using Microsoft.Xrm.Sdk.Client;

    internal class OrganizationService : OrganizationServiceProxy
    {
        private OrganizationService(Uri uri, ClientCredentials clientCredentials) 
            : base(uri, null, clientCredentials, null)
        {
            // Blank
        }

        public static OrganizationService Create(D365Credential credential)
        {
            var uri = new Uri($"https://{credential.InstanceName}.api.{credential.InstanceRegion}.dynamics.com/XRMServices/2011/Organization.svc");

            var credentials = new ClientCredentials()
            {
                UserName =
                {
                    UserName = credential.Username,
                    Password = credential.Password,
                }
            };

            var instance = new OrganizationService(uri, credentials);

            return instance;
        }
    }
}