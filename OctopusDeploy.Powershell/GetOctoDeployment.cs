namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Get, "OctoDeployment", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoDeployment : OctoCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoDeploymentByRelease")]
        public Release Release
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ParameterSetName = "GetOctoDeploymentByReleaseId")]
        public string ReleaseId
        {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        public Contracts.Environment Environment
        {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        public string EnvironmentId
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
            string releaseId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoDeploymentByReleaseId":
                {
                    releaseId = ReleaseId;
                    break;
                }
                case "GetOctoDeploymentByRelease":
                {
                    releaseId = Release.Id;
                    break;
                }
            }

            var client = new RestClient(BaseUri);
            var request = new RestRequest("/api/releases/{releaseId}/deployments", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("releaseId", releaseId);

            var response = await client.ExecuteTaskAsync<OctoResponse<Contracts.Deployment>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                return;
            }

            if (EnvironmentId != null || Environment != null)
            {
                string environmentId = EnvironmentId;
                if (string.IsNullOrWhiteSpace(environmentId) && Environment != null)
                    environmentId = Environment.Id;

                WriteObject(response.Data.Items.Where(i => i.EnvironmentId == environmentId), true);
   
            }
            else
            {
                WriteObject(response.Data);   
            }
        }
    }
}