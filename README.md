# XafAsyncActions# XafAsyncActions


This example show the various ways you can call async operations in XAF actions
and the behaviour of the UI when doing so.


## MyViewController Methods Documentation


### `ActionWithAsyncModifier_Execute(object sender, SimpleActionExecuteEventArgs e)`

#### Description
Executes two asynchronous tasks sequentially and displays their combined results in a success message.

#### Problems
- The operations in the task list might take longer to execute than the time the method takes to finish
- In this case the method will finish, the U.I will continue be responsive 
- but the tasks will still be running in the background and there is not right way to return to the main thread


### `ActionWithAsyncModifierAndOsOperations_Execute(object sender, SimpleActionExecuteEventArgs e)`

#### Description
Executes a series of asynchronous tasks and updates the `Result` property of the `TestObject` instance. Displays a success message upon completion.

#### Problems (same as `ActionWithAsyncModifier_Execute`))
- The operations in the task list might take longer to execute than the time the method takes to finish
- In this case the method will finish, the U.I will continue be responsive 
- but the tasks will still be running in the background and there is not right way to return to the main thread



### `ActionBlockUIThread_Execute(object sender, SimpleActionExecuteEventArgs e)`
Executes a series of tasks synchronously, blocking the UI thread. Displays the results in a success message.

#### Problems
- The U.I will be blocked and so the main thread will be blocked so your application is frozen forever

## Task methods to emulate the async operations
### `Task1()`
Asynchronous task that simulates a delay and returns a result string.

### `Task2()`
Asynchronous task that simulates a delay and returns a result string.


### `GetTaskList()`
Returns a list of asynchronous tasks to be executed.


## Action with background worker methods
### `AsyncActionWithAsyncBackgroundWorker_Execute(object sender, SimpleActionExecuteEventArgs e)`
Queues a list of tasks to run in the background and sets up progress reporting and completion handling.

### `ProcessingDone(Dictionary<int, object> results)`
Handles the completion of all tasks. Intended to run on the UI thread.

### `OnReportProgress(int progress, string status, object result)`
Reports the progress of task execution to the U.I thread. Displays a status message and logs the result.

## Controller methods 
### `OnActivated()`
Called when the controller is activated. Can be used to perform various tasks depending on the target view.

### `OnDeactivated()`
Called when the controller is deactivated. Unsubscribes from events and releases resources.

### `OnViewControlsCreated()`
Called when the view controls are created. Can be used to access and customize the target view control.

## Utility methods 
### `GetInstance()`
Returns the current instance of `TestObject` from the view.

### `ViewCommit()`
Commits changes to the `ObjectSpace` if it is modified.


## MyViewControllerBlazor Methods Documentation

### `ActionWithAsyncModifierAndOsOperations_Execute(object sender, SimpleActionExecuteEventArgs e)`
Shows a loading indicator while the action is executing. Calls the base method to execute a series of asynchronous tasks.

### `ActionWithAsyncModifier_Execute(object sender, SimpleActionExecuteEventArgs e)`
Shows a loading indicator while the action is executing. Calls the base method to execute two asynchronous tasks sequentially.

### `AsyncActionWithAsyncBackgroundWorker_Execute(object sender, SimpleActionExecuteEventArgs e)`
Shows a loading indicator while the action is executing. Calls the base method to queue a list of tasks to run in the background.

### `OnReportProgress(int progress, string status, object result)`
Reports the progress of task execution to the UI thread. Calls the base method to display a status message and log the result.

### `ProcessingDone(Dictionary<int, object> results)`
Handles the completion of all tasks. Calls the base method to run on the UI thread and hides the loading indicator.


