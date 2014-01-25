using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using S1Parser;

namespace S1Nyan.Model.Test
{
    [TestClass]
    public class DataServiceTest
    {
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromCache()
        {
            Mock<IParserFactory> stubPaserFactory = new Mock<IParserFactory>();
            Mock<IStorageHelper> stubStorageHelper = new Mock<IStorageHelper>();

            stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns(new MemoryStream());
            stubPaserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(new List<S1ListItem>());

            DataService service = new DataService(stubStorageHelper.Object, stubPaserFactory.Object);
            var result = await service.UpdateMainListAsync();

            Assert.IsTrue(result is List<S1ListItem>);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromRemote()
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            tcs.SetResult(new MemoryStream());

            Mock<IParserFactory> stubPaserFactory = new Mock<IParserFactory>();
            Mock<IStorageHelper> stubStorageHelper = new Mock<IStorageHelper>();

            stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns<Stream>(null);
            stubPaserFactory.Setup(x => x.GetMainListStream())
                .Returns(tcs.Task);
            stubPaserFactory.Setup(x => x.ParseMainListData(It.IsAny<string>()))
                .Returns(new List<S1ListItem>());

            DataService service = new DataService(stubStorageHelper.Object, stubPaserFactory.Object);

            var result = await service.UpdateMainListAsync();

            Assert.IsTrue(result is List<S1ListItem>);
        }


        [TestMethod]
        public void TestGetThreadListData()
        {

        }

        [TestMethod]
        public void TestThreadData()
        {
            
        }
    }
}
