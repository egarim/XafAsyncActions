using DevExpress.DataAccess.Sql;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafSmartEditors.Razor.NqlDotNet;

namespace XafAsyncActions.Module.Controllers
{
    public class MyViewController : ViewController
    {
        SimpleAction AsyncAction;
        public MyViewController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            AsyncAction = new SimpleAction(this, "Async Action", "View");
            AsyncAction.Execute += AsyncAction_Execute;

        }

        public async Task<string> Task1()
        {
            //HACK this task will run on the background thread
            await Task.Delay(1000);
            return "Result from Task 1:" + Guid.NewGuid().ToString();
        }

        public async Task<string> Task2()
        {
            //HACK this task will run on the background thread
            await Task.Delay(2000);
            return "Result from Task 2:" + Guid.NewGuid().ToString();
        }
        protected virtual void ProcessingDone(Dictionary<int, object> results)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
        }
        protected virtual void AsyncAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            //hack we queue a list of tasks to run in the background
            var tasks = new List<Func<Task<Object>>>
            {
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
            };

            Action<int, string, Object> onProgressChanged = (progress, status, result) =>
            {
                //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
                Debug.WriteLine($"{status} - Result: {result}");
            };

            var worker = new AsyncBackgroundWorker<Object>(
                tasks,//List of task to run
                onProgressChanged,//Progress report action
                result => ProcessingDone(result) //Action to run when all tasks are done
            );

            worker.Start();

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }
}
