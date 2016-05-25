namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using RestSharp;

    [Cmdlet(VerbsCommon.Get, "OctoLibraryVariableSet", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoLibraryVariableSet : OctoCmdlet
    {
        /// <summary>
        /// The version of the release to retrieve 
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "GetOctoLibraryVariableSetByLibraryVariableSetId")]
        public string LibraryVariableSetId
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

            string libraryVariableSetId = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoLibraryVariableSetByLibraryVariableSetId":
                {
                    libraryVariableSetId = LibraryVariableSetId;
                    break;
                }
            }

            var request = new RestRequest("/api/libraryvariablesets/{id}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", libraryVariableSetId);

            var response = await client.ExecuteTaskAsync<Contracts.LibraryVariableSet>(request);
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