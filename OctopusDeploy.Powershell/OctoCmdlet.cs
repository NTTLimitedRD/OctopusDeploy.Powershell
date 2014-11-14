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
        /// The name of the project the release belongs to.
        /// </summary>
        [Parameter]
        public Uri BaseUri
        {
            get
            {
                if (_baseUri != null)
                    return _baseUri;

                var psVar = SessionState.PSVariable.Get("OctoBaseUri");
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
        /// The name of the project the release belongs to.
        /// </summary>
        [Parameter]
        public string ApiKey
        {
            get
            {
                if (_apiKey != null)
                    return _apiKey;

                var psVar = SessionState.PSVariable.Get("OctoApiKey");
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