using System;
using ModelGraphSTD;
using Windows.Storage;

namespace RepositoryUWP
{
    public partial class RepositoryStorageFile : IRepository
    {
        StorageFile _storageFile;

        public RepositoryStorageFile(StorageFile storageFile)
        {
            _storageFile = storageFile;
        }

        #region FullName  =====================================================
        public string FullName => _storageFile.Path;
        public string Name
        {
            get
            {
                var name = _storageFile.Name;
                var index = name.LastIndexOf(".");
                if (index < 0) return name;
                return name.Substring(0, index);
            }
        }
        #endregion

        #region FileFormat  ===================================================
        static Guid _fileFormat_1 = new Guid("D8CA7983-98BC-49CC-B821-432BDA6BADE6");
        static Guid _fileFormat_2 = new Guid("7DD885AE-7004-4ECC-9B9F-B84330326129");
        static Guid _fileFormat_3 = new Guid("069890CE-A832-4BDD-9D7A-54000F88C5C3");
        static Guid _fileFormat_4 = new Guid("7C0620F4-C2E4-4E78-AEFA-5CDC50EDE114");
        static Guid _fileFormat_5 = new Guid("8B9C3519-02FF-4416-9FD9-ED2699AF176E");
        static Guid _fileFormat_6 = new Guid("41489943-94FC-426F-899A-B53A3FF0126A");
        static Guid _fileFormat_7 = new Guid("E660208C-287E-4901-AB45-6BBD71E95359");
        static Guid _fileFormat_8 = new Guid("38AB52EE-46EB-4BBB-94CA-C810AA2EC900");
        #endregion

        #region Mark  =========================================================
        private enum Mark : byte
        {
            ViewXBegin = 1,
            EnumXBegin = 2,
            TableXBegin = 3,
            GraphXBegin = 4,
            QueryXBegin = 5,
            SymbolXBegin = 6,
            ColumnXBegin = 7,
            ComputeXBegin = 8,
            CommandXBegin = 9,
            RelationXBegin = 10,
            GraphParmBegin = 11,
            RelationLinkBegin = 12,

            ViewXEnding = 61,
            EnumXEnding = 62,
            TableXEnding = 63,
            GraphXEnding = 64,
            QueryXEnding = 65,
            SymbolXEnding = 66,
            ColumnXEnding = 67,
            ComputeXEnding = 68,
            CommandXEnding = 69,
            RelationXEnding = 70,
            GraphParmEnding = 71,
            RelationLinkEnding = 72,

            StorageFileEnding = 99,
        }
        #endregion

        #region Flags  ========================================================
        // don't read/write missing or default-value propties
        // these flags indicate which properties were non-default
        const byte BZ = 0;
        const byte B1 = 0x1;
        const byte B2 = 0x2;
        const byte B3 = 0x4;
        const byte B4 = 0x8;
        const byte B5 = 0x10;
        const byte B6 = 0x20;
        const byte B7 = 0x40;
        const byte B8 = 0x80;

        const ushort SZ = 0;
        const ushort S1 = 0x1;
        const ushort S2 = 0x2;
        const ushort S3 = 0x4;
        const ushort S4 = 0x8;
        const ushort S5 = 0x10;
        const ushort S6 = 0x20;
        const ushort S7 = 0x40;
        const ushort S8 = 0x80;
        const ushort S9 = 0x100;
        const ushort S10 = 0x200;
        const ushort S11 = 0x400;
        const ushort S12 = 0x800;
        const ushort S13 = 0x1000;
        const ushort S14 = 0x2000;
        const ushort S15 = 0x4000;
        const ushort S16 = 0x8000;
        #endregion
    }
}
