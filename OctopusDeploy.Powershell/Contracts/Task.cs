namespace DD.Cloud.OctopusDeploy.Powershell.Contracts
{
    using System;

    /// <summary>
    /// The data contract for a task
    /// </summary>
    public class Task
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string Completed
        {
            get;
            set;
        }

        public DateTime QueueTime
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime LastUpdatedTime
        {
            get;
            set;
        }

        public DateTime CompletedTime
        {
            get;
            set;
        }

        public string Duration
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool HasBeenPickedUpByProcessor
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }

        public bool FinishedSuccessfully
        {
            get;
            set;
        }

        public bool HasPendingInterruptions
        {
            get;
            set;
        }

        public bool CanRerun
        {
            get;
            set;
        }

        public bool HasWarningsOrErrors
        {
            get;
            set;
        }
    }
}