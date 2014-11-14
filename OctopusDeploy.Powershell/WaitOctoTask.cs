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

    [Cmdlet(VerbsLifecycle.Wait, "OctoTask", ConfirmImpact = ConfirmImpact.Low)]
    public class WaitOctoTask : OctoCmdlet
    {
        /// <summary>
        /// The id of the task to wait.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "WaitOctoTaskByTaskId")]
        public string TaskId
        {
            get;
            set;
        }

        /// <summary>
        /// The task to wait.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "WaitOctoTaskByTask", ValueFromPipeline = true)]
        public Contracts.Task Task
        {
            get;
            set;
        }

        /// <summary>
        /// The deployment task to wait.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "WaitOctoTaskByDeployment", ValueFromPipeline = true)]
        public Contracts.Deployment Deployment
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

            string taskId = string.Empty;
            switch (ParameterSetName)
            {
                case "WaitOctoTaskByTaskId":
                {
                    taskId = TaskId;
                    break;
                }
                case "WaitOctoTaskByTask":
                {
                    taskId = Task.Id;
                    break;
                }
                case "WaitOctoTaskByDeployment":
                {
                    taskId = Deployment.TaskId;
                    break;
                }
                default:
                {
                    throw new ArgumentException(
                        String.Format(
                            "Unrecognised parameter set: '{0}'.",
                            ParameterSetName
                            )
                        );
                }
            }

            var resourcePath = string.Format("/api/tasks/{0}", taskId);
            var request = new RestRequest(resourcePath, Method.GET);
            request.AddHeader("X-Octopus-ApiKey", ApiKey);

            ProgressRecord progerssRecord;

            int progressCount = 0;
            while (true)
            {
                var response = await client.ExecuteTaskAsync<Contracts.Task>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(new ErrorRecord(new Exception(response.ErrorMessage ?? response.Content), "Failed", ErrorCategory.OpenError, null));
                    break;
                }

                if (response.Data.IsCompleted)
                {
                    WriteObject(response.Data);
                    break;
                }

                progerssRecord = new ProgressRecord(1, "Waiting for deployment to complete.", ProgressCountToStatusDescription(progressCount++));
                progerssRecord.CurrentOperation = "In progress";
                progerssRecord.RecordType = ProgressRecordType.Processing;
                WriteProgress(progerssRecord);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5));
            }

            progerssRecord = new ProgressRecord(1, "Waiting for deployment to complete.", ProgressCountToStatusDescription(progressCount++));
            progerssRecord.CurrentOperation = "In progress";
            progerssRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progerssRecord);
        }

        static string ProgressCountToStatusDescription(int progressCount)
        {
            return new string('*', 1 + progressCount % 10);
        }
    }
}