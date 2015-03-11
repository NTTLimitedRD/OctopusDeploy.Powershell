using System;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoFeeds", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoFeeds : OctoCmdlet
    {
        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);

            var request = new RestRequest("/api/feeds/", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var response = await client.ExecuteTaskAsync<Contracts.Feeds>(request);
        
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
