using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DD.Cloud.OctopusDeploy.Powershell.Contracts;
using Task = System.Threading.Tasks.Task;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoFeeds", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoFeeds : OctoCmdlet
    {
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
            var uri = "/api/feeds/";
            uri += @"?skip=" + Skip;

            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var response = await client.ExecuteTaskAsync<Feeds>(request);
        
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            else
            {
                if (All)
                {
                    var allFeeds = new List<Feed>();
                    allFeeds.AddRange(response.Data.Items);

                    var pages = ((double)response.Data.TotalResults - Skip) / response.Data.ItemsPerPage; // the number of Skipped results might affect the number of pages
                    var numOfPages = (int)Math.Ceiling(pages);
                    for (var i = 1; i < numOfPages; i++) // we'll start from 1 since the first call was already invoked
                    {
                        var percentComplete = (i + 1) * 100 / numOfPages;
                        var tmp = await GetNextFeeds(client, response.Data.ItemsPerPage * i + Skip, percentComplete); // the number of skipped results should be passed to the GetNext method
                        allFeeds.AddRange(tmp.Items);
                    }

                    response.Data.Items = allFeeds;
                    WriteObject(response.Data);
                }
                else
                {
                    WriteObject(response.Data);
                }
            }
        }
                private async Task<Feeds> GetNextFeeds(RestClient client, int skip, int percentComplete)
        {
            var uri = "/api/feeds/";
            uri += @"?skip=" + skip;
            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var response = await client.ExecuteTaskAsync<Feeds>(request);

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
