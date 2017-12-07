using System;
using System.Threading.Tasks;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region PostDataActionRequest  ========================================
        // These methods are called from the ui thread and typically they invoke 
        // some type of change to the dataChefs objects (create, remove, update)

        internal void PostExecuteCommand(ModelCommand command)
        {
            var model = command.Model;
            if (command.Action != null)
                PostRequest(model, () => { command.Action(model); });
            else if (command.Action1 != null)
                PostRequest(model, () => { command.Action1(model, command.Parameter1); });
        }

        internal void PostDataAction(ModelTree model, Action action)
        {
            if (model.IsInvalid) return;

            PostRequest(model, action);
        }
        // Model refresh runs after each execution. We only
        // need to invoke an execution on the background thread.
        public void PostModelRefresh(ModelTree model)
        {
            if (model == null || model.IsInvalid) return;
 
            PostRequest(model, () => { DoNothing(); });
        }
        private void DoNothing() { }

        internal void PostModelSetValue(ModelTree model, string value)
        {
            if (model.IsInvalid) return;

            var item = model.Item1;
            var prop = model.Item2 as Property;
            var oldValue = prop.Value.GetString(item);
            if (IsSameValue(value, oldValue)) return;

            PostRequest(model, () => { SetValue(model, value); });
        }
        internal void PostModelSetIsChecked(ModelTree model, bool value)
        {
            if (model.IsInvalid) return;

            PostModelSetValue(model, value.ToString());
        }
        internal void PostModelSetValueIndex(ModelTree model, int index)
        {
            if (index < 0) return;
            if (model.IsInvalid) return;


            string[] values;
            if (model.Item3 is EnumX x)
            {
                values = GetEnumActualValues(x);
                if (index < values.Length) PostModelSetValue(model, values[index]);
            }
            else if (model.Item3 is EnumZ z)
            {
                var zvals = GetEnumZNames(z);
                if (index < zvals.Length) PostModelSetValue(model, zvals[index]);
            }
        }
        #endregion

        #region PostRequest ===================================================
        //  Called from the ui thread and runs on a background thred

        private async void PostRequest(ModelTree requestingModel, Action requestedDataAction)
        {
            if (requestingModel.IsInvalid) return;

            var action = new ActionRequest(requestingModel, requestedDataAction);
            await Task.Run(() => { ExecuteRequest(action); }); // runs on worker thread 
            //<=== control immediatley returns to the ui thread

            //(some time later the worker task completes and signals the ui thread)

            //===> the ui thread returns here and continues executing the following code            
            foreach (var model in _rootModels) { model.PageDispatch(); }
            
        }

        private void ExecuteRequest(ActionRequest action)
        {
            if (action.IsValid)
            {
                // the dataAction will likey modify the dataChef's objects, 
                // so we can't have multiple threads stepping on each other
                lock (_executionLock)
                {
                    if (IsRootChef)
                    {
                        var model = action.Model;
                        action.Execute();
                    }
                    else
                    {
                        var model = action.Model;
                        action.Execute();
                        CheckChanges();
                    }
                }
            }
        }

        #endregion

        #region ActionRequest  ================================================
        private class ActionRequest
        {
            Action _action;
            ModelTree _model;
            internal ActionRequest(ModelTree model, Action action)
            {
                _action = action;
                _model = model;
            }

            internal ModelTree Model { get { return _model; } }
            internal void Execute() { _action(); _action = null; _model = null; }
            internal bool IsValid { get { return (_action != null && _model != null && !_model.IsInvalid); } }
        }
        #endregion
    }
}
