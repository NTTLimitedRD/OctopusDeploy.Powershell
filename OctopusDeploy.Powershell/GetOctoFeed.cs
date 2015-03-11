using System;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoFeed", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoFeed : OctoCmdlet
    {
        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "GetOctoFeedsByFeed")]
        public Contracts.Feed Feed
        {
            get;
            set;
        }

        /// <summary>
        /// The project the release belongs to.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "GetOctoFeedsByFeedId")]
        public string FeedId
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

            string feedId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoFeedsByFeedId":
                    {
                        feedId = FeedId;
                        break;
                    }
                case "GetOctoFeedsByFeed":
                    {
                        feedId = Feed.Id;
                        break;
                    }
            }

            var request = new RestRequest("/api/feeds/{id}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", feedId);
            var response = await client.ExecuteTaskAsync<Contracts.Feed>(request);
        
            if (response.StatusCode != HttpStatusCode.OK)
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
