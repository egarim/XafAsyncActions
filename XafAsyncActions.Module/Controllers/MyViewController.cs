using DevExpress.DataAccess.Sql;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XafAsyncActions.Module.BusinessObjects;
using XafSmartEditors.Razor.NqlDotNet;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace XafAsyncActions.Module.Controllers
{
    public class MyViewController : ViewController
    {
        SimpleAction ActionWithAsyncModifierAndOsOperations;
        SimpleAction ActionWithAsyncModifier;
        SimpleAction ActionBlockUIThread;
        SimpleAction AsyncActionWithAsyncBackgroundWorker;
        public MyViewController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            AsyncActionWithAsyncBackgroundWorker = new SimpleAction(this, nameof(AsyncActionWithAsyncBackgroundWorker), "View");
            AsyncActionWithAsyncBackgroundWorker.Execute += AsyncActionWithAsyncBackgroundWorker_Execute;

            ActionBlockUIThread = new SimpleAction(this, nameof(ActionBlockUIThread), "View");
            ActionBlockUIThread.Execute += ActionBlockUIThread_Execute;

            ActionWithAsyncModifier = new SimpleAction(this, nameof(ActionWithAsyncModifier), "View");
            ActionWithAsyncModifier.Execute += ActionWithAsyncModifier_Execute;

            ActionWithAsyncModifierAndOsOperations = new SimpleAction(this, nameof(ActionWithAsyncModifierAndOsOperations), "View");
            ActionWithAsyncModifierAndOsOperations.Execute += ActionWithAsyncModifierAndOsOperations_Execute;
            
            this.TargetObjectType = typeof(TestObject);
            this.TargetViewType= ViewType.DetailView;



        }
        protected TestObject GetInstance()
        {
            return this.View.CurrentObject as TestObject;
        }
        protected void ViewCommit()
        {
            if(this.View.ObjectSpace.IsModified)
            {
                this.View.ObjectSpace.CommitChanges();
            }
        }
        protected virtual async void ActionWithAsyncModifierAndOsOperations_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var Instance = GetInstance();
            var Tasks = GetTaskList();
            StringBuilder Results = new StringBuilder();
            foreach (var item in Tasks)
            {
                var CurrentResult = await item.Invoke();
                Results.AppendLine(CurrentResult.ToString());
            }
            Instance.Result= Results.ToString();
            ViewCommit();
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            options.Message = Instance.Result;
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            Application.ShowViewStrategy.ShowMessage(options);
        }
        protected virtual async void ActionWithAsyncModifier_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var Tasks = GetTaskList();
            StringBuilder Results = new StringBuilder();
            foreach (var item in Tasks)
            {
                var CurrentResult = await item.Invoke();
                Results.AppendLine(CurrentResult.ToString());
            }
            MessageOptions options = new MessageOptions();
            options.Duration = 2000;
            options.Message = Results.ToString();
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            Application.ShowViewStrategy.ShowMessage(options);
        }
        protected virtual void ActionBlockUIThread_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //HACK This action will block the UI thread and never recover, just an example of what not to do
            var Tasks = GetTaskList();
            StringBuilder Results=new StringBuilder();
            foreach (var item in Tasks)
            {
                Results.AppendLine(item.Invoke().GetAwaiter().GetResult().ToString());
            }

            MessageOptions options = new MessageOptions();
            options.Duration = 2000;
            options.Message = Results.ToString();
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        public async Task<string> Task1()
        {
            //HACK this task will run on the background thread
            await Task.Delay(1500);
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
        protected List<Func<Task<Object>>> GetTaskList()
        {
            return new List<Func<Task<Object>>>
            {
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task1(); },
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task1(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task2(); },
                async () => { return await Task1(); },
            };
        }
        protected virtual void OnReportProgress(int progress,string status, object result)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
            MessageOptions options = new MessageOptions();
            options.Duration = 2000;
            options.Message = status;
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            Application.ShowViewStrategy.ShowMessage(options);

            Debug.WriteLine($"{status} - Result: {result}");
        }
        protected virtual void AsyncActionWithAsyncBackgroundWorker_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            //hack we queue a list of tasks to run in the background
            var tasks = GetTaskList();

            //Action trigger each time a task is done and progress is notified
            Action<int, string, Object> onProgressChanged = OnReportProgress;


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
