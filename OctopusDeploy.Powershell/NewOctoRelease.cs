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
        /// <summary>
        /// The version of the release to retrieve 
        /// </summary>
        [Parameter(Mandatory = true)]
        [Parameter(ParameterSetName = "NewOctoReleaseByProject")]
        [Parameter(ParameterSetName = "NewOctoReleaseByProjectId")]
        public Version Version
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

            var packageTemplates = await GetPackageTemplatesAsync(client, deploymentProecessId);

            var specificPackageVersions = SpecificPackageVersions.Cast<DictionaryEntry>().ToDictionary(k => k.Key.ToString(), v => new Version(v.Value.ToString()));
            var tasks = packageTemplates.Select(packageTemplate => GetLatestPackageFromTemplatesAsync(client, packageTemplate, specificPackageVersions));
            var packages = await Task.WhenAll(tasks);

            string packagesDescription = string.Join(System.Environment.NewLine,
                                                     packages.Select(package => string.Format("{0} {1}", package.PackageId, package.Version)));

            var request = new RestRequest("/api/releases", Method.POST);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddJsonBody(new Contracts.Release
            {
                ProjectId = projectId,
                Version = Version.ToString(),
                SelectedPackages = packages.ToList(),
                ReleaseNotes = string.IsNullOrEmpty(ReleaseNotes) ? packagesDescription : string.Join(System.Environment.NewLine, ReleaseNotes, packagesDescription)
            });

            var response = await client.ExecuteTaskAsync<Contracts.Release>(request);
            if (response.StatusCode != HttpStatusCode.Created)
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

        async Task<IEnumerable<Contracts.PackageTemplate>> GetPackageTemplatesAsync(IRestClient client, string deploymentProcessId)
        {
            var resourcePath = "/api/deploymentprocesses/{deployment-process-id}/template";
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("deployment-process-id", deploymentProcessId);

            var response = await client.ExecuteTaskAsync<Contracts.DeploymentProcessTemplate>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                ThrowTerminatingError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));

            return response.Data.Packages;
        }

        async Task<Contracts.StepPackage> GetLatestPackageFromTemplatesAsync(IRestClient client, Contracts.PackageTemplate packageTemplate, IDictionary<string, Version> specificPackageVersions = null)
        {
            if (specificPackageVersions != null && specificPackageVersions.ContainsKey(packageTemplate.NuGetPackageId))
            {
                return new StepPackage{ PackageId = packageTemplate.NuGetPackageId, StepName = packageTemplate.StepName, Version = specificPackageVersions[packageTemplate.NuGetPackageId].ToString() };
            }

            const string resourcePath = "/api/feeds/{feed-id}/packages";
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("feed-id", packageTemplate.NuGetFeedId);
            request.AddQueryParameter("packageIds", packageTemplate.NuGetPackageId);

            var response = await client.ExecuteTaskAsync<List<Contracts.Package>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                ThrowTerminatingError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));

            var package = response.Data.FirstOrDefault();

            if (package == null)
            {
                ThrowTerminatingError(new ErrorRecord(
                        new Exception(string.Format("Package is missing from feed. PackageId:{0} Feed:{1}", packageTemplate.NuGetPackageId, packageTemplate.NuGetFeedName)),
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