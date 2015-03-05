using System;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoReleases", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoReleases : OctoCmdlet
    {
        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoReleasesByProject")]
        public Contracts.Project Project
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoReleasesByProjectId")]
        public string ProjectId
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
            var client = new RestClient(BaseUri);

            string projectId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoReleasesByProjectId":
                {
                    projectId = ProjectId;
                    break;
                }
                case "GetOctoReleasesByProject":
                {
                    projectId = Project.Id;
                    break;
                }
            }

            var request = new RestRequest("/api/projects/{projectId}/releases/", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("projectId", projectId);
            var response = await client.ExecuteTaskAsync<Contracts.Releases>(request);
        
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                WriteObject(response.Data);
            }
        }
    }
}
