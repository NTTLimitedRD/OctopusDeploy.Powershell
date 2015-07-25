namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// The base class for all Octo cmdlets
    /// </summary>
    abstract public class OctoCmdlet : AsyncCmdlet
    {
        Uri _baseUri;
        string _apiKey;

        /// <summary>
        /// The Base Uri for Octopus API.
        /// </summary>
        [Parameter]
        public Uri BaseUri
        {
            get
            {
                if (_baseUri != null)
                    return _baseUri;

                var psVar = SessionState.PSVariable.Get("global:OctoBaseUri");
                if (psVar != null)
                    return psVar.Value as Uri;

                return null;
            }

            set
            {
                _baseUri = value;
            }
        }

        /// <summary>
        /// The API key to call the Rest API with.
        /// </summary>
        [Parameter]
        public string ApiKey
        {
            get
            {
                if (_apiKey != null)
                    return _apiKey;

                var psVar = SessionState.PSVariable.Get("global:OctoApiKey");
                if (psVar != null)
                    return psVar.Value as string;

                return null;
            }

            set
            {
                _apiKey = value;
            }
        }
    }
}