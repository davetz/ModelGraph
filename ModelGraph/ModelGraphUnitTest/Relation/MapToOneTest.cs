using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelGraphLibrary;

namespace ModelGraphUnitTest.Relation
{
    [TestClass]
    public class MapToOneTest
    {
        [TestMethod]
        public void MapToOneWriteReadTest()
        {
            RunTest(2, 1);
            RunTest(8, 3);

            void RunTest(int tblCount, int rowCount)
            {
                Assert.IsTrue(tblCount > 1);
                Assert.IsTrue(rowCount > 0);

                (var ts, var mp) = SetupTestData(tblCount, rowCount);

                var tbls = ts.Items;

                var N = tblCount / 2;
                for (int i = 0; i < N; i += 2)
                {
                    var tb1 = tbls[i];
                    var tb2 = tbls[i + 1];
                    var rows1 = tb1.Items;
                    var rows2 = tb2.Items;

                    for (int j = 0; j < rowCount; j++)
                    {
                        Assert.IsTrue(mp.ContainsLink(rows1[j], rows2[j]));
                    }
                }
            }
        }

        (StoreOf<TableX>, MapToOne<RowX>) SetupTestData(int tblCount, int rowCount)
        {
            var ts = new StoreOf<TableX>(null, Trait.TableXStore, System.Guid.Empty, tblCount);
            for (int i = 0; i < tblCount; i++)
            {
                var tb = new TableX(ts);
                for (int j = 0; j < rowCount; j++)
                {
                    new RowX(tb);
                }
            }
            var tbls = ts.Items;
            var mp = new MapToOne<RowX>(tblCount * rowCount);

            var N = tblCount / 2;
            for (int i = 0; i < N; i += 2)
            {
                var tb1 = tbls[i];
                var tb2 = tbls[i + 1];
                var rows1 = tb1.Items;
                var rows2 = tb2.Items;

                for (int j = 0; j < rowCount; j++)
                {
                    mp.SetLink(rows1[j], rows2[j]);
                }
            }
            return (ts, mp);
        }
    }
}
