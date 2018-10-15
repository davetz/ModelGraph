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

        private bool ShowItemIndex;

        #region DataChef  =====================================================
        internal Chef(IRepository repository = null) : base(null, Trait.DataChef, Guid.Empty, 0)
        {
            Initialize();

            Repository = repository;

            if (repository == null)
                _newChefNumber = (_newChefCount += 1);
            else
                Repository.Read(this);
        }
        internal override void Release()
        {
            Repository = null;
            GraphParms = null;
            Property_Enum = null;
            _itemIdentity = null;
            _localize = null;

            ReleaseEnums();
            ReleaseStores();
            ReleaseRelations();
            ReleaseProperties();
            ReleaseModelActions();

            base.Release();
        }
        #endregion

        #region RootModels  ===================================================
        private RootModel PrimaryRootModel;
        private List<RootModel> _rootModels = new List<RootModel>(10);
        internal void AddRootModel(RootModel root)
        {
            if (PrimaryRootModel == null) PrimaryRootModel = root;
            _rootModels.Add(root);
        }
        internal void RemoveRootModel(RootModel root)
        {
            _rootModels.Remove(root);
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

            InitializeModelActions();
        }
        #endregion
    }
}
