using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class TaskValueHelperTests
    {
        [TestMethod]
        public async Task Run()
        {
            var any = AsyncMethod();
            var value = await TaskValueHelper.GetPossiblyAsyncValue(any);

            Assert.AreEqual(3, value);
        }

        private async Task<int> AsyncMethod() => 3;
    }
}