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

    [Cmdlet(VerbsCommon.Get, "OctoProject", ConfirmImpact = ConfirmImpact.Low)]
    public class GetOctoProject : OctoCmdlet
    {
        /// <summary>
        /// The name of the project to retrieve.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "GetOctoProjectByName")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The project group to filter.
        /// </summary>
        [Parameter(Mandatory = false)]
        public ProjectGroup ProjectGroup
        {
            get;
            set;
        }

        /// <summary>
        /// The id of the project group to filter.
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ProjectGroupId
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
            string filterByName = string.Empty;
            switch (ParameterSetName)
            {
                case "GetOctoProjectByName":
                {
                    filterByName = Name;
                    break;
                }
            }

            var client = new RestClient(BaseUri);
            var request = new RestRequest("/api/projects", Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            var response = await client.ExecuteTaskAsync<OctoResponse<Project>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                return;
            }

            string projectGroupId = ProjectGroupId;
            if (string.IsNullOrWhiteSpace(projectGroupId) && ProjectGroup != null)
                projectGroupId = ProjectGroup.Id;

            if (ListAvailable.IsPresent)
            {
                WriteObject(
                    response.Data.Items.Where(
                        i => string.IsNullOrWhiteSpace(projectGroupId) ||
                             (!string.IsNullOrWhiteSpace(projectGroupId) &&
                              string.Compare(projectGroupId, i.ProjectGroupId, StringComparison.InvariantCultureIgnoreCase) == 0)),
                    true);
            }
            else
            {
                WriteObject(
                    response.Data.Items
                        .FirstOrDefault(
                            i => (string.IsNullOrWhiteSpace(projectGroupId) && string.Compare(i.Name, filterByName, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                                 (!string.IsNullOrWhiteSpace(projectGroupId) && string.Compare(i.Name, filterByName, StringComparison.InvariantCultureIgnoreCase) == 0 && string.Compare(i.ProjectGroupId, projectGroupId, StringComparison.InvariantCultureIgnoreCase) == 0)
                        ));
            }
        }
    }
}