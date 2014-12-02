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

    [Cmdlet(VerbsCommon.Get, "OctoDeploymentProcess", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoDeploymentProcess : OctoCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoDeploymentProcessById")]
        public string DeploymentProcessId
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
            string deploymentProcessId = String.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoDeploymentProcessById":
                {
                    deploymentProcessId = DeploymentProcessId;
                    break;
                }
            }

            var client = new RestClient(BaseUri);
            var request = new RestRequest("/api/deploymentprocesses/{deploymentProcessId}/template", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("deploymentProcessId", deploymentProcessId);

            var response = await client.ExecuteTaskAsync<DeploymentProcess>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                return;
            }

            WriteObject(response.Data);
        }
    }
}