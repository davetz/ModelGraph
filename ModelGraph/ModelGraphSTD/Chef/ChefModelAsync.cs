using System;
using System.Threading.Tasks;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        #region PostRequest  ==================================================
        // These methods are called from the ui thread and typically they invoke 
        // some type of change to the dataChefs objects (create, remove, update)
        internal void PostAction(ItemModel model, Action action)
        {
            if (model.IsInvalid) return;

            PostModelRequest(model, action);
        }
        internal void PostRefresh(ItemModel model)
        {
            PostModelRequest(model, () => { });
        }
        internal void PostCommand(ModelCommand command)
        {
            var model = command.Model;
            if (command.Action != null)
                PostModelRequest(model, () => { command.Action(model); });
            else if (command.Action1 != null)
                PostModelRequest(model, () => { command.Action1(model, command.Parameter1); });
        }
        internal void PostRefreshViewList(RootModel root, ItemModel select, int scroll, ChangeType change)
        {
            if (root.ControlType == ControlType.AppRootChef) return;

            root.SelectModel = select;
            PostModelRequest(root, () => RefreshViewFlatList(root, scroll, change));
        }
        internal void PostSetValue(ItemModel model, bool value)
        {
            PostSetValue(model, value.ToString());
        }
        internal void PostSetValue(ItemModel model, string value)
        {
            if (model.IsInvalid) return;

            var item = model.Item;
            var prop = model.Aux1 as Property;
            var oldValue = prop.Value.GetString(item);
            if (IsSameValue(value, oldValue)) return;

            PostModelRequest(model, () => {SetValue(model, value); });
        }
        internal void PostSetValue(ItemModel model, int index)
        {
            if (index < 0) return;
            if (model.IsInvalid) return;


            string[] values;
            if (model.Aux2 is EnumX x)
            {
                values = GetEnumActualValues(x);
                if (index < values.Length) PostSetValue(model, values[index]);
            }
            else if (model.Aux2 is EnumZ z)
            {
                var zvals = GetEnumZNames(z);
                if (index < zvals.Length) PostSetValue(model, zvals[index]);
            }
        }
        #endregion

        #region ExecuteRequest ================================================
        //  Called from the ui thread and runs on a background thred

        private async void PostModelRequest(ItemModel model, Action action)
        {
            await Task.Run(() => { ExecuteRequest(new ModelRequest(model, action)); }); // runs on worker thread 
            //<=== control immediatley returns to the ui thread

            //(some time later the worker task completes and signals the ui thread)

            //===> the ui thread returns here and continues executing the following code
            var rootModels = _rootModels.ToArray(); // get a copy of the root model list
            foreach (var root in rootModels) { root.PageDispatch(); }
        }
        private void ExecuteRequest(ModelRequest request)
        {
            // the dataAction will likey modify the dataChef's objects, 
            // so we can't have multiple threads stepping on each other
            lock (this)
            {
                request.Execute();

                if (!IsRootChef) CheckChanges();

                foreach (var root in _rootModels)
                {
                    if (root.HasFlatList) RefreshViewFlatList(root);
                    root.UIRequestRefreshModel();
                }
            }
        }

        class ModelRequest
        {
            ItemModel _model;
            readonly Action _action;
            internal ModelRequest(ItemModel model, Action action)
            {
                _model = model;
                _action = action;
            }

            internal void Execute()
            {/*
                make sure the requested model action is still valid, because it is posible that
                a preceeding action in the queue could have invalidated this model 
             */
                if (_model != null && _action != null && _model.Item != null && _model.Item.IsValid) _action();
            }

            internal RootModel RootModel => _model.GetRootModel();
        }
        #endregion
    }
}
