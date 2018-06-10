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
        public static ItemModel DragDropSource; 

        private object _executionLock = new object(); // only one thread may modify the data

        #region RootChef  =====================================================
        internal Chef() : base(null, Trait.RootChef, Guid.Empty, 10)
        {
            Owner = this;
            Initialize_RootChef_X();
        }
        #endregion

        #region DataChef  =====================================================
        internal Chef(Chef rootChef, IRepository repository = null) : base(rootChef, Trait.DataChef, Guid.Empty, 0)
        {
            Initialize();

            Repository = repository;

            rootChef.Add(this);
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
            if (Owner is Chef rootChef && rootChef != this)
            {
                rootChef.Remove(this);
                rootChef.MajorDelta += 1;

                rootChef.PrimaryRootModel.PageDispatch();
                _rootModels.Clear();
            }
        }
        #endregion

        #region RootModels  ===================================================
        private RootModel PrimaryRootModel;
        private List<RootModel> _rootModels = new List<RootModel>(10);
        internal void AddRootModel(RootModel root)
        {
            if (PrimaryRootModel == null) PrimaryRootModel = root;

            if (root.Item is Graph g) g.AddRootModel(root);

            MajorDelta += 1;
            _rootModels.Add(root);
        }
        internal void RemoveRootModel(RootModel root)
        {
            if (root.Item is Graph g) g.RemoveRootModel(root);
            MajorDelta += 1;

            _rootModels.Remove(root);
        }
        #endregion

        #region GetRootChef  ==================================================
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

            Initialize_ModelActions();
        }
        #endregion
    }
}
