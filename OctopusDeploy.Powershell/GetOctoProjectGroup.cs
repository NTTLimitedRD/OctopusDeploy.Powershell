namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Get, "OctoProjectGroup", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoProjectGroup : OctoCmdlet
    {
        /// <summary>
        /// The name of the project group.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoProjectGroupByName")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A switch to specify to list all project groups available.
        /// </summary>
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
                case "GetOctoProjectGroupByName":
                {
                    filterByName = Name;
                    break;
                }
            }

            var client = new RestClient(BaseUri);
            var request = new RestRequest("/api/projectgroups/all", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            var response = await client.ExecuteTaskAsync<List<ProjectGroup>>(request);
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