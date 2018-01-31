using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelGraphSTD
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var rootChef = new Chef();
            var dataChef = new Chef(rootChef);
        }
    }
}
