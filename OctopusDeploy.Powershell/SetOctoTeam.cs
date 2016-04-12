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

    [Cmdlet(VerbsCommon.Set, "OctoTeam", ConfirmImpact = ConfirmImpact.Medium)]
    public class SetOctoTeam : OctoCmdlet
    {
        /// <summary>
        /// The id of the Team to update.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "SetOctoTeamById")]
        public string TeamId
        {
            get;
            set;
        }
 
        /// <summary>
        /// The list of users to set for the team to update.
        /// </summary>
        [Parameter(Mandatory = true)]
        public List<string> TeamUsers
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

            var team = await GetTeamAsync(client, TeamId);
            if (team.Id != null)
            {
                var request = new RestRequest("/api/teams/{id}", Method.PUT);
                request.AddUrlSegment("id", team.Id);
                request.AddHeader("X-Octopus-ApiKey", ApiKey);
               
                request.AddJsonBody(new Contracts.Team
                {
                    Name = team.Name,
                    EnvironmentIds = team.EnvironmentIds,
                    ProjectIds = team.ProjectIds,
                    UserRoleIds = team.UserRoleIds,
                    MemberUserIds = TeamUsers
                });

                var response = await client.ExecuteTaskAsync<Team>(request);
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

        private async Task<Team> GetTeamAsync(RestClient client, string teamId)
        {
            var request = new RestRequest("/api/teams/{id}", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);
            request.AddUrlSegment("id", teamId);

            var response = await client.ExecuteTaskAsync<Team>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
            }
            return response.Data;
        }
    }
}