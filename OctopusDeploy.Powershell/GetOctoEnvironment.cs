namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoEnvironment", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoEnvironment : OctoCmdlet
    {
        /// <summary>
        /// The name of the project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoEnvironmentByName")]
        public string Name
        {
            get;
            set;
        }

        [Parameter(Mandatory = false, ParameterSetName = "ListAvailable")]
        public SwitchParameter ListAvailable
        {
            get;
            set;
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            string filterByName = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoEnvironmentByName":
                {
                    filterByName = Name;
                    break;
                }
            }

            var client = new RestClient(BaseUri);
            var request = new RestRequest("/api/environments/all", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            var response = await client.ExecuteTaskAsync<List<Contracts.Environment>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                return;
            }

            if (ListAvailable.IsPresent)
            {
                WriteObject(response.Data, true);
            }
            else
            {
                WriteObject(response.Data.FirstOrDefault(i => string.Compare(i.Name, filterByName, StringComparison.InvariantCultureIgnoreCase) == 0));
            }
        }
    }
}