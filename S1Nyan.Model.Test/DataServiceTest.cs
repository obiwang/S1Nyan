using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using S1Parser;
using S1Parser.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S1Nyan.Model.Test
{
    [TestClass]
    public class DataServiceTest
    {
        private Mock<IParserFactory> _stubParserFactory;
        private Mock<IStorageHelper> _stubStorageHelper;
        private TaskCompletionSource<Stream> _tcs;
        private Stream _testStream;
        private List<S1ListItem> _testList;

        [TestInitialize]
        public void Setup()
        {
            _testStream = new MemoryStream();
            _testList = new List<S1ListItem>();
            _stubParserFactory = new Mock<IParserFactory>();
            _stubStorageHelper = new Mock<IStorageHelper>();
            _tcs = new TaskCompletionSource<Stream>();
            _tcs.SetResult(_testStream);
        }

        #region UpdateMainListAsync Tests

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromCache()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns(_testStream);
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(new List<S1ListItem>());

            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);
            var result = await service.UpdateMainListAsync();

            Assert.IsTrue(result is List<S1ListItem>);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromCachedData()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns(_testStream);
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(_testList);
            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);

            var result = await service.UpdateMainListAsync();

            _stubParserFactory.Verify(x => x.ParseMainListData(_testStream));
            Assert.AreEqual(_testList, result);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_FromRemote()
        {
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Returns<Stream>(null);
            _stubParserFactory.Setup(x => x.GetMainListStream())
                .Returns(_tcs.Task);
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<string>()))
                .Returns(_testList);
            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);

            var result = await service.UpdateMainListAsync();

            _stubParserFactory.Verify(x => x.ParseMainListData(It.IsAny<string>()));
            _stubStorageHelper.Verify(x => x.WriteToLocalCache(It.IsAny<string>(), It.IsAny<string>()));
            Assert.AreEqual(_testList, result);
        }

        [TestMethod]
        public async Task TestUpdateMainListAsync_WithInvalidData()
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
                Assert.IsInstanceOfType(e, typeof(S1UserException));
            }
        }

        #endregion

        #region GetMainListCache Tests

        [TestMethod]
        public void TestGetMainListCache_FromCache()
        {
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(_testList);
            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);

            var result = service.GetMainListCache();
            _stubStorageHelper.Verify(x => x.ReadFromLocalCache(It.IsAny<string>(), -1));
            Assert.AreEqual(_testList, result);
        }

        [TestMethod]
        public void TestGetMainListCache_WithNoCache()
        {
            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);

            var result = service.GetMainListCache();
            _stubStorageHelper.Verify(x => x.ReadFromAppResource(It.IsAny<string>()));
        }

        [TestMethod]
        public void TestGetMainListCache_WithException()
        {
            _stubParserFactory.Setup(x => x.ParseMainListData(It.IsAny<Stream>()))
                .Returns(_testList);
            _stubStorageHelper.Setup(x => x.ReadFromLocalCache(It.IsAny<string>(), It.IsAny<double>()))
                .Throws(new Exception());

            var service = new DataService(_stubStorageHelper.Object, _stubParserFactory.Object);

            var result = service.GetMainListCache();
            _stubStorageHelper.Verify(x => x.ReadFromAppResource(It.IsAny<string>()));
            Assert.AreEqual(_testList, result);
        }

        #endregion

    }
}
