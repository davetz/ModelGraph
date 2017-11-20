using System;
using System.Collections.Generic;
using ModelGraph.Helpers;
using Windows.Storage;

namespace ModelGraph.Internals
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
        internal Chef(Chef rootChef, StorageFile file = null) : base(rootChef, Trait.DataChef, Guid.Empty, 0)
        {
            _selfReferenceModel = new RootModel(this);

            Initialize();

            _modelingFile = file;

            rootChef.Append(this);

            if (file == null)
                _newChefNumber = (_newChefCount += 1);
            //else
            //    ReadModelDataFile(file);
        }
        #endregion

        #region RootModels  ===================================================
        private List<RootModel> _rootModels = new List<RootModel>(10);
        private RootModel _selfReferenceModel;
        internal void AddRootModel(RootModel root)
        {
            var g = root.Item1 as Graph;
            if (g != null) g.AddRootModel(root);
            MajorDelta += 1;
            _rootModels.Add(root);
        }
        internal void RemoveRootModel(RootModel root)
        {
            var g = root.Item1 as Graph;
            if (g != null) g.RemoveRootModel(root);
            MajorDelta += 1;
            _rootModels.Remove(root);
            PostModelRefresh(_selfReferenceModel);
        }
        #endregion

        #region DragDrop  =====================================================
        internal ItemModel DragDropSource
        {
            get { return GetRootChef().DragDropModel; }
            set { GetRootChef().DragDropModel = value; }
        }
        protected ItemModel DragDropModel;

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

        #region StorageFile  ==================================================
        private StorageFile _modelingFile;
        public StorageFile ModelingFile => _modelingFile;
        private void SaveStorageFile()
        {
            if (_modelingFile != null)
            {
                //WriteModelDataFile(_modelingFile);
                CongealChanges();
            }
        }
        private void SaveAsStorageFile(StorageFile file)
        {
            _modelingFile = file;
            if (_modelingFile != null)
            {
                //WriteModelDataFile(_modelingFile);
                CongealChanges();
            }
        }
        private string GetLongStorageFileName()
        {
            if (_modelingFile == null) return NullStorageFileName;

            return _modelingFile.Name;
        }
        private string GetShortStorageFileName()
        {
            if (_modelingFile == null) return NullStorageFileName;

            var name = _modelingFile.Name;
            var index = name.LastIndexOf(".");
            if (index < 0) return name;
            return name.Substring(0, index);
        }
        private string NullStorageFileName => $"{GetNameKey(Trait.NewModel).GetLocalized()} #{_newChefNumber}";
        #endregion
    }
}
