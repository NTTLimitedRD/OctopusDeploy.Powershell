namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using RestSharp.Deserializers;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Get, "OctoTask", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoTask : OctoCmdlet
    {
        /// <summary>
        /// The deployment that the task belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoTaskByDeployment")]
        public Deployment Deployment
        {
            get;
            set;
        }

        /// <summary>
        /// The id of the deployment the task belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoTaskByDeploymentId")]
        public string DeploymentId
        {
            get;
            set;
        }

        /// <summary>
        /// The id of the task to retrieve.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoTaskByTaskId")]
        public string TaskId
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
            BaseUri = new Uri("http://175.184.202.15/");
            ApiKey = "API-IMFWZ31LUKHNH1Y8WLS4KF15DY0";

            var client = new RestClient(BaseUri);

            string taskId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoTaskByTaskId":
                {
                    taskId = TaskId;
                    break;
                }
                case "GetOctoTaskByDeployment":
                {
                    taskId = Deployment.TaskId;
                    break;
                }
                case "GetOctoTaskByDeploymentId":
                {
                    var deployment = await GetDeploymentAsync(client, DeploymentId);
                    taskId = deployment.TaskId;
                    break;
                }
            }

            var request = new RestRequest("/api/tasks/{taskId}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("taskId", taskId);

            var response = await client.ExecuteTaskAsync<Contracts.Task>(request);

            WriteObject(response.Data);
        }

        async Task<Deployment> GetDeploymentAsync(IRestClient client, string deploymentId)
        {
            var resourcePath = string.Format("/api/deployments/{0}", deploymentId);
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            var response = await client.ExecuteTaskAsync<Contracts.Deployment>(request);
            return response.Data;
        }
    }
}