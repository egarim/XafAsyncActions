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
        protected async override void AsyncAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            //HACK show loading indicator
            loading.Hold("Loading");
            //Execute the base action on the agnostic module
            base.AsyncAction_Execute(sender, e);
        

        }
        protected async override void ProcessingDone(Dictionary<int, object> results)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
            base.ProcessingDone(results);
            //HACK hide loading indicator
            loading.Release("Loading");
           
        }
    }
}
