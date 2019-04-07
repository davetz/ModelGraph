using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Chef : StoreOf<Store>
    {
        static int _newChefCount;
        private int _newChefNumber;
        public static ItemModel DragDropSource; 

        private bool ShowItemIndex;

        #region Constructor  ==================================================
        internal Chef(IRepository repository = null) : base(null, Trait.DataChef, Guid.Empty, 0)
        {
            Initialize();

            Repository = repository;

            if (repository == null)
                _newChefNumber = (_newChefCount += 1);
            else
                Repository.Read(this);
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
            if (_rootModels is null) return;

            _rootModels.Remove(root);
            if (_rootModels.Count == 0) Release();
        }
        #endregion

        #region Initialize  ===================================================
        private void Initialize()
        {
            ClearItemErrors();
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

        #region Release  ======================================================
        internal override void Release()
        {
            Repository = null;
            GraphParms = null;
            Property_Enum = null;
            _itemIdentity = null;
            _localize = null;
            _rootModels = null;

            ReleaseEnums();
            ReleaseStores();
            ReleaseRelations();
            ReleaseProperties();
            ReleaseModelActions();

            base.Release();
        }
        #endregion
    }
}
