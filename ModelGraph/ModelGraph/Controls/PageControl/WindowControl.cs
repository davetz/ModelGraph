using System;
using ModelGraph.Internals;
using ModelGraph.Services;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace ModelGraph
{
    // A custom event that fires whenever the secondary view is ready to be closed.
    public delegate void ViewReleasedHandler(Object sender, EventArgs e);

    // A ModelPageControl is instantiated for every secondary view window. 
    // (StartInUse) increments _refCount, (StopInUse) decrements it.
    // When the reference count drops to zero, the secondary window is closed.
    public sealed class WindowControl
    {
        event ViewReleasedHandler _viewReleasedHandler;
        ModelPageService _pageService;
        CoreDispatcher _dispatcher;
        CoreWindow _window;
        int _refCount = 0;
        int _viewId;
        bool _released = false;
        ModelRoot _rootModel;

        public int Id => _viewId;
        public ModelRoot RootModel => _rootModel;

        internal static WindowControl CreateForCurrentView(ModelPageService pageService, ModelRoot model)
        {
            return new WindowControl(pageService, CoreWindow.GetForCurrentThread(), model);
        }

        internal void CloseModelPages(Chef chef) => _pageService.CloseModelPages(chef);
        internal void CreateModelPage(ModelRoot model) => _pageService.CreateModelPage(model);

        #region Constructor  ==================================================
        private WindowControl(ModelPageService pageService, CoreWindow window, ModelRoot rootModel)
        {
            _pageService = pageService;
            _dispatcher = window.Dispatcher;
            _window = window;
            _rootModel = rootModel;
            _viewId = ApplicationView.GetApplicationViewIdForWindow(_window);

            RegisterForEvents();
        }
        #endregion

        #region ViewReleasedHandler  ==========================================
        // Used to store pubicly registered events under the protection of a lock
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
            StopInUse();
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

        #region StartInUse  ===================================================
        internal int StartInUse()
        {
            bool releasedCopy = false;
            int refCountCopy = 0;

            // This method is called from several different threads
            // (each view lives on its own thread)
            lock (this)
            {
                releasedCopy = _released;
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

        #region StopInUse  ====================================================
        internal int StopInUse()
        {
            int refCountCopy = 0;
            bool releasedCopy = false;

            lock (this)
            {
                releasedCopy = _released;
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
