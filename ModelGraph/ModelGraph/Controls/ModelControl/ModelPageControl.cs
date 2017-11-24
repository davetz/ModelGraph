using ModelGraph.Internals;
using ModelGraph.Services;
using System;
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace ModelGraph
{
    // A custom event that fires whenever the secondary view is ready to be closed. You should
    // clean up any state (including deregistering for events) then close the window in this handler
    public delegate void ViewReleasedHandler(Object sender, EventArgs e);

    // A ModelPageControl is instantiated for every secondary view. ModelPageControl's reference count
    // keeps track of when the secondary view thinks it's in usem and when the main view is interacting with the secondary view (about to show
    // it to the user, etc.) When the reference count drops to zero, the secondary view is closed.
    public sealed class ModelPageControl
    {
        ModelPageService _pageService;
        CoreDispatcher _dispatcher;
        CoreWindow _window;
        int _refCount = 0;
        int _viewId;
        bool _released = false;
        RootModel _model;

        internal static ModelPageControl CreateForCurrentView(ModelPageService pageService, RootModel model)
        {
            return new ModelPageControl(pageService, CoreWindow.GetForCurrentThread(), model);
        }

        #region Constructor  ==================================================
        private ModelPageControl(ModelPageService pageService, CoreWindow window, RootModel model)
        {
            _pageService = pageService;
            _dispatcher = window.Dispatcher;
            _window = window;
            _model = model;
            _model.PageControl = this;
            _viewId = ApplicationView.GetApplicationViewIdForWindow(_window);

            RegisterForEvents();
        }
        #endregion

        #region Dispatch  =====================================================
        internal void Dispatch(UIRequest req)
        {

        }
        #endregion

        #region Property  =====================================================
        public int Id => _viewId;
        public CoreDispatcher Dispatcher => _dispatcher;
        internal object Owner => _model.Chef;
        #endregion

        #region ViewReleasedHandler  ==========================================
        // Used to store pubicly registered events under the protection of a lock
        event ViewReleasedHandler _viewReleasedHandler;

        public event ViewReleasedHandler Released
        {
            add
            {
                bool releasedCopy = false;
                lock (this)
                {
                    releasedCopy = _released;
                    if (!_released)
                    {
                        _viewReleasedHandler += value;
                    }
                }

                if (releasedCopy)
                {
                    throw new InvalidOperationException("This view is being disposed");
                }
            }

            remove
            {
                lock (this)
                {
                    _viewReleasedHandler -= value;
                }
            }
        }
        #endregion

        #region Events  =======================================================
        private void RegisterForEvents()
        {
            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }
        private void UnregisterForEvents()
        {
            ApplicationView.GetForCurrentView().Consolidated -= ViewConsolidated;
        }
        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs e)
        {
            StopViewInUse();
        }
        private void FinalizeRelease()
        {
            bool justReleased = false;
            lock (this)
            {
                if (_refCount == 0)
                {
                    justReleased = true;
                    _released = true;
                }
            }

            // This assumes that released will never be made false after it
            // it has been set to true
            if (justReleased)
            {
                UnregisterForEvents();
                _viewReleasedHandler(this, null);
            }
        }
        #endregion

        #region StartViewInUse  ===============================================
        internal int StartViewInUse()
        {
            bool releasedCopy = false;
            int refCountCopy = 0;

            // This method is called from several different threads
            // (each view lives on its own thread)
            lock (this)
            {
                releasedCopy = this._released;
                if (!_released)
                {
                    refCountCopy = ++_refCount;
                }
            }

            if (releasedCopy)
            {
                throw new InvalidOperationException("This view is being disposed");
            }

            return refCountCopy;
        }
        #endregion

        #region StopViewInUse  ================================================
        internal int StopViewInUse()
        {
            int refCountCopy = 0;
            bool releasedCopy = false;

            lock (this)
            {
                releasedCopy = this._released;
                if (!_released)
                {
                    refCountCopy = --_refCount;
                    if (refCountCopy == 0)
                    {
                        var task = _dispatcher.RunAsync(CoreDispatcherPriority.Low, FinalizeRelease);
                        _pageService.RemoveModelPage(this);
                    }
                }
            }

            if (releasedCopy)
            {
                throw new InvalidOperationException("This view is being disposed");
            }
            return refCountCopy;
        }
        #endregion

    }
}
