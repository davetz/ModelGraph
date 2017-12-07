using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    Chef is the data coordinator of this application.
    It is the owner of the data and orchastrates all changes.
 */
    public partial class Chef : StoreOf<Store>
    {
        static int _newChefCount;
        private int _newChefNumber;

        private object _executionLock = new object(); // only one thread may modify the data

        #region RootChef  =====================================================
        internal Chef() : base(null, Trait.RootChef, Guid.Empty, 10)
        {
            Owner = this;
        }
        #endregion

        #region DataChef  =====================================================
        internal Chef(Chef rootChef, IRepository repository = null) : base(rootChef, Trait.DataChef, Guid.Empty, 0)
        {
            Initialize();

            Repository = repository;

            rootChef.Append(this);
            rootChef.MajorDelta += 1;

            if (repository == null)
                _newChefNumber = (_newChefCount += 1);
            else
                Repository.Read(this);
        }
        #endregion

        #region Close  ========================================================
        public void Close()
        {
            var rootChef = Owner as Chef;
            if (rootChef != null && rootChef != this)
            {
                rootChef.Remove(this);
                rootChef.MajorDelta += 1;

                rootChef.PrimaryRootModel.PageDispatch();
                _rootModels.Clear();
            }
        }
        #endregion

        #region RootModels  ===================================================
        private ModelRoot PrimaryRootModel;
        private List<ModelRoot> _rootModels = new List<ModelRoot>(10);
        internal void AddRootModel(ModelRoot root)
        {
            if (PrimaryRootModel == null) PrimaryRootModel = root;

            var g = root.Item1 as Graph;
            if (g != null) g.AddRootModel(root);

            MajorDelta += 1;
            _rootModels.Add(root);
        }
        internal void RemoveRootModel(ModelRoot root)
        {
            var g = root.Item1 as Graph;
            if (g != null) g.RemoveRootModel(root);
            MajorDelta += 1;

            _rootModels.Remove(root);
            PostModelRefresh(PrimaryRootModel);
        }
        #endregion

        #region DragDrop  =====================================================
        internal ModelTree DragDropSource
        {
            get { return GetRootChef().DragDropModel; }
            set { GetRootChef().DragDropModel = value; }
        }
        protected ModelTree DragDropModel;

        private Chef GetRootChef()
        {
            var chef = this;
            while (!chef.IsRootChef) { chef = chef.Owner as Chef; }
            return chef;
        }
        #endregion

        #region Initialize  ===================================================
        private void Initialize()
        {
            ResetError();
            InitializeItemIdentity();
            InitializeGraphParams();

            InitializeStores();
            InitializeRelations();

            InitializeEnums();
            InitializeProperties();

            InitializeReferences();
        }
        #endregion
    }
}
