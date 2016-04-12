namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Threading.Tasks;
    using Contracts;
    using RestSharp;
    using Task = System.Threading.Tasks.Task;
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.Get, "OctoTeam", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoTeam : OctoCmdlet
    {
        /// <summary>
        /// The Team Id.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoTeamByTeamId")]
        public string TeamId
        {
            get;
            set;
        }

        /// <summary>
        /// The Teamname of the Team to retrieve.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoTeamByTeamName")]
        public string TeamName
        {
            get;
            set;
        }

        [Parameter(Mandatory = false, ParameterSetName = "ListAvailable")]
        public SwitchParameter ListAvailable
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

            string teamId = string.Empty;
            string filterByTeamName = string.Empty;

            switch (ParameterSetName)
            {
                case "GetOctoTeamByTeamId":
                {
                        teamId = TeamId;
                        break;
                }
                case "GetOctoTeamByTeamName":
                    {
                        filterByTeamName = TeamName;
                        break;
                    }
            }
            var uri = "/api/teams";
            if (teamId != string.Empty)
            {
                uri += "/{id}";
                var request = new RestRequest(uri, Method.GET);
                request.AddUrlSegment("id", teamId);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
                var response = await client.ExecuteTaskAsync<Team>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                }
                else
                {
                    WriteObject(response.Data);
                }
            }
            else if (ListAvailable.IsPresent || filterByTeamName != string.Empty)
            {
                uri += "/all";
                var request = new RestRequest(uri, Method.GET);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
                var response = await client.ExecuteGetTaskAsync<List<Team>>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                }
                else
                {
                    if (filterByTeamName == string.Empty)
                    {
                        WriteObject(response.Data);
                    }
                    else
                    {
                        var allTeams = response;
                        WriteObject(allTeams.Data
                       .FirstOrDefault(
                           i => (string.Compare(i.Name, filterByTeamName, StringComparison.InvariantCultureIgnoreCase) == 0)));
                    }
                }
            }
        }
    }
}
