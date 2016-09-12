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

    [Cmdlet(VerbsCommon.New, "OctoRelease", ConfirmImpact = ConfirmImpact.Medium)]
    public class NewOctoRelease : OctoCmdlet
    {
	    public NewOctoRelease()
	    {
			SpecificPackageVersions = new Hashtable();
	    }

        /// <summary>
        /// The version of the release to retrieve 
        /// </summary>
        [Parameter(Mandatory = true)]
        [Parameter(ParameterSetName = "NewOctoReleaseByProject")]
        [Parameter(ParameterSetName = "NewOctoReleaseByProjectId")]
        public string Version
        {
            get;
            set;
        }

        [Parameter(ParameterSetName = "NewOctoReleaseByProject")]
        [Parameter(ParameterSetName = "NewOctoReleaseByProjectId")]
        public Hashtable SpecificPackageVersions
        {
            get;
            set;
        }

            /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "NewOctoReleaseByProject")]
        public Contracts.Project Project
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "NewOctoReleaseByProjectId")]
        public string ProjectId
        {
            get;
            set;
        }

        /// <summary>
        /// The release belongs to.
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ReleaseNotes
        {
            get;
            set;
        }

        /// <summary>
        /// The release goes to the specified channel.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "NewOctoReleaseByProject")]
        [Parameter(Mandatory = false, ParameterSetName = "NewOctoReleaseByProjectId")]
        public string ChannelName
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
            string deploymentProecessId = string.Empty;
            switch (ParameterSetName)
            {
                case "NewOctoReleaseByProjectId":
                {
                    projectId = ProjectId;
                    var project = await GetProjectAsync(client, ProjectId);
                    deploymentProecessId = project.DeploymentProcessId;

                    break;
                }
                case "NewOctoReleaseByProject":
                {
                    projectId = Project.Id;
                    deploymentProecessId = Project.DeploymentProcessId;

                    break;
                }
            }
            var channel = await GetProjectChannelAsync(client, projectId, ChannelName);
            var channelId = channel != null ? channel.Id : string.Empty;
            var packageTemplates = await GetPackageTemplatesAsync(client, deploymentProecessId, channelId);


	        var specificPackageVersions = SpecificPackageVersions.Cast<DictionaryEntry>()
																	.ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

            var tasks = packageTemplates.Select(packageTemplate => GetLatestPackageFromTemplatesAsync(client, packageTemplate, specificPackageVersions));
            var packages = await Task.WhenAll(tasks);

            string packagesDescription = string.Join(System.Environment.NewLine,
                                                     packages.OrderBy(package => package.PackageId).Select(package => string.Format("- {0} {1}", package.PackageId, package.Version)));
           
            var request = new RestRequest("/api/releases", Method.POST);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var releaseNotes = string.IsNullOrEmpty(ReleaseNotes)
                ? packagesDescription
                : string.Join(System.Environment.NewLine + System.Environment.NewLine, ReleaseNotes, packagesDescription);
            request.AddJsonBody(new Contracts.Release
            {
                ChannelId = channelId,
                ProjectId = projectId,
                Version = Version,
                SelectedPackages = packages.ToList(),
                ReleaseNotes = releaseNotes
            });

            var response = await client.ExecuteTaskAsync<Contracts.Release>(request);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                ThrowTerminatingError(new ErrorRecord(
                           new Exception(
                               string.Format("Unable to create a release. ProjectId:{0} ChannelId:{1} Version:{2}, selected packages: {3}, Release Notes {4}, Api: {5}, Api Error:{6}",
                                   projectId,
                                   channelId,
                                   Version,
                                   packagesDescription,
                                   releaseNotes,
                                   response?.ResponseUri.ToString(),
                                   response.ErrorMessage ?? response.Content)),
                           "ApiError",
                           ErrorCategory.OpenError,
                           null
                       ));
            }
            else
            {                
                WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", response.ResponseUri, response.StatusCode, response.Content));
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
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new Exception(
                            string.Format(
                                "Unable to get project. projectId:{0}, Api: {1}, Api Error:{2}",
                                projectId,
                                response?.ResponseUri.ToString(),
                                response.ErrorMessage ?? response.Content)),
                        "ApiError",
                        ErrorCategory.OpenError,
                        null
                        ));
            }
            
            WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", response.ResponseUri, response.StatusCode, response.Content));

            return response.Data;
        }


        async Task<Contracts.Channel> GetProjectChannelAsync(IRestClient client, string projectId, string channelName)
        {
            Contracts.Channel channel = null;

            if (string.IsNullOrWhiteSpace(channelName))
                return null;

            var resourcePath = "/api/projects/{id}/channels";
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", projectId);

            var response = await client.ExecuteTaskAsync<Contracts.PagedResult<Channel>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new Exception(
                            string.Format(
                                "Unable to get project channel. projectId:{0} channelName:{1}, Api: {2}, Api Error:{3}",
                                projectId,
                                channelName,
                                response?.ResponseUri.ToString(),
                                response.ErrorMessage ?? response.Content)),
                        "ApiError",
                        ErrorCategory.OpenError,
                        null
                        ));
            }            
            WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", response.ResponseUri, response.StatusCode, response.Content));

            channel = response.Data.Items.FirstOrDefault(ch => string.Compare(ch.Name, channelName.Trim(' '), StringComparison.OrdinalIgnoreCase) == 0);

            if(channel == null)
                ThrowTerminatingError(new ErrorRecord(
                            new Exception(
                                string.Format("Channel is missing from the project. ProjectId:{0} ChannelName:{1}",
                                    projectId,
                                    channelName)),
                            "MissingChannel",
                            ErrorCategory.ObjectNotFound,
                            null
                        ));


            return channel;
        }

        async Task<IEnumerable<Contracts.PackageTemplate>> GetPackageTemplatesAsync(IRestClient client, string deploymentProcessId, string channelId)
        {
            var resourcePath = "/api/deploymentprocesses/{deployment-process-id}/template";
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("deployment-process-id", deploymentProcessId);
            if(!string.IsNullOrWhiteSpace(channelId))
                request.AddQueryParameter("channel", channelId);

            var response = await client.ExecuteTaskAsync<Contracts.DeploymentProcessTemplate>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new Exception(
                            string.Format(
                                "Unable to get deploymentprocess template. deploymentProcessId:{0} channelId:{1}, Api: {2}, Api Error:{3}",
                                deploymentProcessId,
                                channelId,
                                response?.ResponseUri.ToString(),
                                response.ErrorMessage ?? response.Content)),
                        "ApiError",
                        ErrorCategory.OpenError,
                        null
                        ));                
            }            
            WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", response.ResponseUri, response.StatusCode, response.Content));

            return response.Data.Packages;
        }

        async Task<Contracts.StepPackage> GetLatestPackageFromTemplatesAsync(IRestClient client, Contracts.PackageTemplate packageTemplate, IDictionary<string, string> specificPackageVersions = null)
        {            
            RestRequest request;
            // The Validation should force indexing of this package (for older versions)
            if (specificPackageVersions != null && specificPackageVersions.ContainsKey(packageTemplate.NuGetPackageId))
            {
                // Validate the package if it exists, by just getting the notes for the package
                // if the package doesnt exists, this will return 404
                const string packageNoteApiPath = "/api/feeds/{feed-id}/packages/notes";
                request = new RestRequest(packageNoteApiPath, Method.GET);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
                request.AddUrlSegment("feed-id", packageTemplate.NuGetFeedId);
                request.AddQueryParameter("packageId", packageTemplate.NuGetPackageId);
                request.AddQueryParameter("version", specificPackageVersions[packageTemplate.NuGetPackageId]);

                var noteResponse = await client.ExecuteGetTaskAsync(request);

                if (noteResponse.StatusCode != HttpStatusCode.OK)
                {
                    ThrowTerminatingError(new ErrorRecord(
                            new Exception(
                                string.Format("Package is missing from feed. PackageId:{0} Version:{1} Feed:{2}, Api: {3}, Api Error:{4}",
                                    packageTemplate.NuGetPackageId,
                                    specificPackageVersions[packageTemplate.NuGetPackageId],
                                    packageTemplate.NuGetFeedName,
                                    noteResponse ?.ResponseUri.ToString(),
                                    noteResponse.ErrorMessage ?? noteResponse.Content)),
                            "MissingPackage",
                            ErrorCategory.ObjectNotFound,
                            null
                        ));
                }
                WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", noteResponse.ResponseUri, noteResponse.StatusCode, noteResponse.Content));

                return new StepPackage
                {
                    PackageId = packageTemplate.NuGetPackageId,
                    StepName = packageTemplate.StepName,
                    Version = specificPackageVersions[packageTemplate.NuGetPackageId]
                };
            }

            const string resourcePath = "/api/feeds/{feed-id}/packages";
            request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("feed-id", packageTemplate.NuGetFeedId);
            request.AddQueryParameter("packageIds", packageTemplate.NuGetPackageId); // Just returns the latest release version of this package

            var response = await client.ExecuteTaskAsync<List<Contracts.Package>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                ThrowTerminatingError(
                    new ErrorRecord(
                        new Exception(
                            string.Format("Package is missing from feed. PackageId:{0} Feed:{1}, Api: {2}, Api Error:{3}", 
                            packageTemplate.NuGetPackageId,
                            packageTemplate.NuGetFeedId,
                            response.ResponseUri.ToString(),
                            response.ErrorMessage ?? response.Content)),
                        "MissingPackage",
                        ErrorCategory.OpenError,
                        null));
            
            WriteVerbose(string.Format("Api: {0}, Http Status:{1} Api Response:{2}", response.ResponseUri, response.StatusCode, response.Content));


            var package = response.Data.FirstOrDefault();

            if (package == null)
            {
                ThrowTerminatingError(new ErrorRecord(
                        new Exception(
							string.Format("Package is missing from feed. PackageId:{0} Feed:{1}", 
								packageTemplate.NuGetPackageId,
								packageTemplate.NuGetFeedName)),
                        "MissingPackage",
                        ErrorCategory.ObjectNotFound, 
                        null 
                    ));
            }

            WriteVerbose(string.Format("{0} {1}", package.NuGetPackageId, package.Version));

            return new StepPackage { PackageId = packageTemplate.NuGetPackageId, StepName = packageTemplate.StepName, Version = package.Version };
        }
    }
}