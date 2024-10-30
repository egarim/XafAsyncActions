using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Utils;
using Microsoft.JSInterop;
using XafAsyncActions.Module.Controllers;

namespace XafAsyncActions.Blazor.Server.Controllers
{
    //HACK https://supportcenter.devexpress.com/ticket/details/t1080389/notify-progress-of-action-execution-changing-the-loading-progress-icon
    public class MyViewControllerBlazor:MyViewController
    {

        IJSRuntime jsRuntime;
        IServiceProvider serviceProvider;
        ILoadingIndicatorProvider loading;
        protected override void OnActivated()
        {
            base.OnActivated();
            IServiceProvider serviceProvider = ((BlazorApplication)Application).ServiceProvider;
            jsRuntime = (IJSRuntime)serviceProvider.GetService(typeof(IJSRuntime));
            loading = serviceProvider.GetService(typeof(ILoadingIndicatorProvider)) as ILoadingIndicatorProvider;

          
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

        }

        //The idea here is to show a loading indicator while the action is executing
        protected async override void ActionWithAsyncModifierAndOsOperations_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            loading.Hold("Loading");//this will not happen because the action is async and the U.I thread will NOT be blocked
            //during this execution the U.I thread will not be blocked, and the user can interact with the object or navigate away,this is not what we want
            base.ActionWithAsyncModifierAndOsOperations_Execute(sender, e);
            loading.Release("Loading");//this will not happen because the action is async and the U.I thread will NOT be blocked
        }
        protected async override void ActionWithAsyncModifier_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            loading.Hold("Loading");//this will not happen because the action is async and the U.I thread will NOT be blocked
            base.ActionWithAsyncModifier_Execute(sender, e);
            loading.Release("Loading");//this will not happen because the action is async and the U.I thread will NOT be blocked
        }

        #region AsyncActionWithAsyncBackgroundWorker

        protected async override void AsyncActionWithAsyncBackgroundWorker_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            //HACK show loading indicator
            loading.Hold("Loading");
            //Execute the base action on the agnostic module
            base.AsyncActionWithAsyncBackgroundWorker_Execute(sender, e);

        }
        protected override void OnReportProgress(int progress, string status, object result)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
            base.OnReportProgress(progress, status, result);
        }
        protected async override void ProcessingDone(Dictionary<int, object> results)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
            base.ProcessingDone(results);
            //HACK hide loading indicator
            loading.Release("Loading");

        }
        #endregion
    }
}
