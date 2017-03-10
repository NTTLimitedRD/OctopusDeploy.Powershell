namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.Set, "OctoLifecycle", ConfirmImpact = ConfirmImpact.Medium)]
    public class SetOctoLifecycle : OctoCmdlet
    {
        /// <summary>
        /// The id of the Lifecycle to update.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "SetOctoLifecycleById")]
        public string LifecycleId
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the phase to update.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string PhaseName
        {
            get;
            set;
        }

        /// <summary>
        /// The environment id to add for the Lifecycle to update.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string EnvironmentId
        {
            get;
            set;
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter AppendAutoDeploymentTargets
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

            var Lifecycle = await GetLifecycleAsync(client, LifecycleId);
            if (Lifecycle.Id != null)
            {
                var Phases = Lifecycle.Phases;
                foreach (var existingPhase in Phases)
                {
                    if(existingPhase.Name == PhaseName)
                    {
                        if (AppendAutoDeploymentTargets)
                        {
                            existingPhase.AutomaticDeploymentTargets.Add(EnvironmentId);
                        }
                        else
                        {
                            existingPhase.OptionalDeploymentTargets.Add(EnvironmentId);
                        }
                        break;
                    }

                }
                var request = new RestRequest("/api/Lifecycles/{id}", Method.PUT);
                request.AddUrlSegment("id", Lifecycle.Id);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
               
                request.AddJsonBody(new Lifecycle
                {
                    Name = Lifecycle.Name,
                    Description = Lifecycle.Description,
                    Phases = Phases
                });

                var response = await client.ExecuteTaskAsync<Lifecycle>(request);
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

        private async Task<Lifecycle> GetLifecycleAsync(RestClient client, string LifecycleId)
        {
            var request = new RestRequest("/api/Lifecycles/{id}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", LifecycleId);

            var response = await client.ExecuteTaskAsync<Lifecycle>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            return response.Data;
        }
    }
}