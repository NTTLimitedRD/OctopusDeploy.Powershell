using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using System.Threading.Tasks;
using DD.Cloud.OctopusDeploy.Powershell.Contracts;
using Task = System.Threading.Tasks.Task;

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
        /// Support pagination by skipping parameter.
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Skip
        {
            get;
            set;
        }

        /// <summary>
        /// When using this option all the releases will be retrieved - not just the items of 1 page.
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter All
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
            var uri = "/api/projects/{projectId}/releases/";
            uri += @"?skip=" + Skip;
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("projectId", projectId);
            var response = await client.ExecuteTaskAsync<Releases>(request);
        
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                if (All)
                {
                    var allReleases = new List<Release>();
                    allReleases.AddRange(response.Data.Items);

                    var pages = ((double)response.Data.TotalResults -Skip) / response.Data.ItemsPerPage; // the number of Skipped results might affect the number of pages
                    var numOfPages = (int)Math.Ceiling(pages);
                    for (var i = 1; i < numOfPages; i++) // we'll start from 1 since the first call was already invoked
                    {
                        var percentComplete = (i + 1)*100/numOfPages;
                        var tmp = await GetNextReleases(client, projectId, response.Data.ItemsPerPage * i + Skip, percentComplete); // the number of skipped results should be passed to the GetNext method
                        allReleases.AddRange(tmp.Items);
                    }

                    response.Data.Items = allReleases;
                    WriteObject(response.Data);
                }
                else
                {
                    WriteObject(response.Data);
                }
            }
        }

        private async Task<Releases> GetNextReleases(RestClient client, string projectId, int skip, int percentComplete)
        {
            var uri = "/api/projects/{projectId}/releases/";
            uri += @"?skip=" + skip;
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("projectId", projectId);
            var response = await client.ExecuteTaskAsync<Releases>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            UpdateProgress(percentComplete);
            return response.Data;
        }
        private void UpdateProgress(int percentComplete)
        {
            var myprogress = new ProgressRecord(1, "Getting more results...", String.Format("{0}% Complete", percentComplete))
            {
                PercentComplete = percentComplete
            };
            WriteProgress(myprogress);
        }
    }
}
