using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoRelease", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoRelease : OctoCmdlet
    {
        /// <summary>
        /// The version of the release to retrieve 
        /// </summary>
        [Parameter(Mandatory = true)]
        [Parameter(ParameterSetName = "GetOctoReleaseByProject")]
        [Parameter(ParameterSetName = "GetOctoReleaseByProjectId")]
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoReleaseByProject")]
        public Contracts.Project Project
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoReleaseByProjectId")]
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
                case "GetOctoReleaseByProjectId":
                {
                    projectId = ProjectId;
                    break;
                }
                case "GetOctoReleaseByProject":
                {
                    projectId = Project.Id;
                    break;
                }
            }


            var request = new RestRequest("/api/projects/{projectId}/releases/{version}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("projectId", projectId);
            request.AddUrlSegment("version", Version);

            var response = await client.ExecuteTaskAsync<Contracts.Release>(request);
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
