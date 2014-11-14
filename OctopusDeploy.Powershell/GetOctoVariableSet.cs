namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoVariableSet", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoVariableSet : OctoCmdlet
    {
        /// <summary>
        /// The version of the release to retrieve 
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "GetOctoVariableSetByVariableSetId")]
        public string VariableSetId
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoVariableSetByProject")]
        public Contracts.Project Project
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "GetOctoVariableSetByProjectId")]
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

            string variableSetId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoVariableSetByVariableSetId":
                {
                    variableSetId = VariableSetId;
                    break;
                }
                case "GetOctoVariableSetByProject":
                {
                    variableSetId = Project.VariableSetId;
                    break;
                }
                case "GetOctoVariableSetByProjectId":
                {
                    var project = await GetProjectAsync(client, ProjectId);
                    variableSetId = project.VariableSetId;
                    break;
                }
            }

            var request = new RestRequest("/api/variables/{variableset-id}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("variableset-id", variableSetId);

            var response = await client.ExecuteTaskAsync<Contracts.VariableSet>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                WriteObject(response.Data);
            }
        }

        async Task<Contracts.Project> GetProjectAsync(IRestClient client, string projectId)
        {
            var resourcePath = "/api/projects/{id}";
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", projectId);

            var response = await client.ExecuteTaskAsync<Contracts.Project>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                ThrowTerminatingError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));

            return response.Data;
        }

    }
}