namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Set, "OctoRelease", ConfirmImpact = ConfirmImpact.Medium)]
    public class SetOctoRelease : OctoCmdlet
    {
        /// <summary>
        /// The version of the release to update 
        /// </summary>
        [Parameter(Mandatory = true)]
        [Parameter(ParameterSetName = "SetOctoReleaseByProject")]
        [Parameter(ParameterSetName = "SetOctoReleaseByProjectId")]
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "SetOctoReleaseByProject")]
        public Contracts.Project Project
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "SetOctoReleaseByProjectId")]
        public string ProjectId
        {
            get;
            set;
        }

        /// <summary>
        /// The release notes to set.
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ReleaseNotes
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

            string projectId = string.Empty;

            switch (ParameterSetName)
            {
                case "SetOctoReleaseByProjectId":
                {
                    projectId = ProjectId;
                    break;
                }
                case "SetOctoReleaseByProject":
                {
                    projectId = Project.Id;
                    break;
                }
            }

            var release = await GetReleaseAsync(client, Version, projectId);
            if (release.Id != null)
            {
                var request = new RestRequest("/api/releases/{id}", Method.PUT);
                request.AddUrlSegment("id", release.Id);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);

                request.AddJsonBody(new Contracts.Release
                {
                    ProjectId = projectId,
                    Id = release.Id,
                    Version = Version,
                    SelectedPackages = release.SelectedPackages,
                    ReleaseNotes = string.IsNullOrEmpty(ReleaseNotes) ? release.ReleaseNotes : ReleaseNotes
                });

                var response = await client.ExecuteTaskAsync<Contracts.Release>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed",
                        ErrorCategory.OpenError, null));
                }
                else
                {
                    WriteObject(response.Data);
                }
            }
        }

        private async Task<Release> GetReleaseAsync(RestClient client, string version, string projectId)
        {
            var request = new RestRequest("/api/projects/{projectId}/releases/{version}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("projectId", projectId);
            request.AddUrlSegment("version", version);

            var response = await client.ExecuteTaskAsync<Contracts.Release>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            return response.Data;
        }
    }
}