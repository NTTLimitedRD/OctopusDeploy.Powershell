namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Threading.Tasks;

    [Cmdlet(VerbsCommon.Set, "OctoDefaults", ConfirmImpact = ConfirmImpact.Low)]
    public class SetOctoDefaults : PSCmdlet
    {
        /// <summary>
        /// The base url of the octopus deploy server.
        /// </summary>
        [Parameter]
        public Uri BaseUri
        {
            get;
            set;
        }

        /// <summary>
        /// The api key to use for authentication with the octopus deploy.
        /// </summary>
        [Parameter]
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Processing the command
        /// </summary>
        protected override void BeginProcessing()
        {
            if (BaseUri != null)
            {
                SessionState.PSVariable.Set("OctoBaseUri", BaseUri);
            }

            if (ApiKey != null)
            {
                SessionState.PSVariable.Set("OctoApiKey", ApiKey);
            }
        }
    }
}