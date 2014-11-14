namespace DD.Cloud.OctopusDeploy.Powershell
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    /// <summary>
    ///		Base class for Cmdlets that run asynchronously.
    /// </summary>
    /// <remarks>
    ///		Inherit from this class if your Cmdlet needs to use <c>async</c> / <c>await</c> functionality.
    /// </remarks>
    public abstract class AsyncCmdlet
        : PSCmdlet
    {
        /// <summary>
        ///		A pre-completed task to be returned from async methods that are not asynchronous.
        /// </summary>
        protected static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        ///		Initialise the <see cref="AsyncCmdlet"/>.
        /// </summary>
        protected AsyncCmdlet()
        {
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet pre-processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task BeginProcessingAsync()
        {
            return CompletedTask;
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task ProcessRecordAsync()
        {
            return CompletedTask;
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet post-processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task EndProcessingAsync()
        {
            return CompletedTask;
        }

        /// <summary>
        ///		Perform Cmdlet processing.
        /// </summary>
        protected sealed override void ProcessRecord()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () =>
                    ProcessRecordAsync()
            );
        }

        /// <summary>
        ///		Perform Cmdlet pre-processing.
        /// </summary>
        protected sealed override void BeginProcessing()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () =>
                    BeginProcessingAsync()
            );
        }

        /// <summary>
        ///		Perform Cmdlet post-processing.
        /// </summary>
        protected sealed override void EndProcessing()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () =>
                    EndProcessingAsync()
            );
        }
    }
}
