namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.New, "OctoDeployment", ConfirmImpact = ConfirmImpact.Medium)]
    public class NewOctoDeployment : OctoCmdlet
    {
        /// <summary>
        /// The release to deploy.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "NewOctoDeploymentByRelease")]
        public Contracts.Release Release
        {
            get;
            set;
        }

        /// <summary>
        /// The release to deploy.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "NewOctoDeploymentByReleaseId")]
        public string ReleaseId
        {
            get;
            set;
        }

        /// <summary>
        /// The environmnet to deploy the release to.
        /// </summary>
        [Parameter(ParameterSetName = "NewOctoDeploymentByRelease")]
        [Parameter(ParameterSetName = "NewOctoDeploymentByReleaseId")]
        public Contracts.Environment Environment
        {
            get;
            set;
        }

        /// <summary>
        /// The environmnet to deploy the release to.
        /// </summary>
        [Parameter(ParameterSetName = "NewOctoDeploymentByRelease")]
        [Parameter(ParameterSetName = "NewOctoDeploymentByReleaseId")]
        public string EnvironmentId
        {
            get;
            set;
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);

            string releaseId = string.Empty;
            string environmentId = string.Empty;
            switch (ParameterSetName)
            {
                case "NewOctoDeploymentByReleaseId":
                {
                    releaseId = ReleaseId;

                    break;
                }
                case "NewOctoDeploymentByRelease":
                {
                    releaseId = Release.Id;

                    break;
                }
            }

            var request = new RestRequest("/api/deployments", Method.POST);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddJsonBody(new
            {
                ReleaseId = releaseId,
                EnvironmentId = EnvironmentId ?? Environment.Id,
            });

            var response = await client.ExecuteTaskAsync<Contracts.Deployment>(request);
            if (response.StatusCode != HttpStatusCode.Created)
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