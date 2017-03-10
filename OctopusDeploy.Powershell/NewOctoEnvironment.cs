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

    [Cmdlet(VerbsCommon.New, "OctoEnvironment", ConfirmImpact = ConfirmImpact.Medium)]
    public class NewOctoEnvironment : OctoCmdlet
    {
        public NewOctoEnvironment()
        {
            Name = "New Environment";
            Description = "New Description";
            UseGuidedFailure = true;
        }

        /// <summary>
        /// The new Environment's name
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The new Environment's display name
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Is the new Environment an active Environment
        /// </summary>
        [Parameter(Mandatory = false)]
        public bool UseGuidedFailure
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

            var request = new RestRequest("/api/Environments", Method.POST);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddJsonBody(new Contracts.Environment
            {
                Name = Name,
                Description = Description,
                UseGuidedFailure = UseGuidedFailure
            });

            var response = await client.ExecuteTaskAsync<Contracts.Environment>(request);
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