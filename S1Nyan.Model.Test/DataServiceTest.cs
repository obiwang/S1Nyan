using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.Model.Test
{
    [TestClass]
    public class DataServiceTest
    {
        private Mock<IParserFactory> _stubParserFactory;
        private Mock<IStorageHelper> _stubStorageHelper;
        private TaskCompletionSource<Stream> _tcs;

        [TestInitialize]
        public void Setup()
        {
            _stubParserFactory = new Mock<IParserFactory>();
            _stubStorageHelper = new Mock<IStorageHelper>();
            _tcs = new TaskCompletionSource<Stream>();
            _tcs.SetResult(new MemoryStream());
        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromCache()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns(new MemoryStream());
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(new List<S1ListItem>());

            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);
            var result = await service.UpdateMainListAsync();

            Assert.IsTrue(result is List<S1ListItem>);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsyncFromRemote()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns<Stream>(null);
            _stubParserFactory.Setup(x => x.GetMainListStream())
                .Returns(_tcs.Task);
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<string>()))
                .Returns(new List<S1ListItem>());

            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);
            var result = await service.UpdateMainListAsync();

            Assert.IsTrue(result is List<S1ListItem>);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsyncWithInvalidData()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns<Stream>(null);
            _stubParserFactory.Setup(x => x.GetMainListStream())
                .Returns(_tcs.Task);
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<string>()))
                .Throws(new S1UserException(""));

            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);
            try
            {
                await service.UpdateMainListAsync();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof (S1UserException));
            }
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
