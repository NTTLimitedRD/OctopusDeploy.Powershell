using System;
using System.Threading.Tasks;

namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Net;
    using RestSharp;
    using Contracts;
    using Task = System.Threading.Tasks.Task;

    [Cmdlet(VerbsCommon.Get, "OctoLifecycle", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoLifecycle : OctoCmdlet
    {
        /// <summary>
        /// The name of the lifecycle to retrieve.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoLifecycleByName")]
        public string Name
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
        ///
        protected override async Task ProcessRecordAsync()
        {
            var client = new RestClient(BaseUri);
            var uri = "/api/lifecycles/";

            var request = new RestRequest(uri, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            var response = await client.ExecuteTaskAsync<Lifecycles>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            var allLifecyclesResponse = response.Data;


            foreach (var lifecycle in allLifecyclesResponse.Items)
            {
                if (lifecycle.Name == Name)
                {
                    WriteObject(lifecycle);
                    break;
                }
            }
        }
    }
}
