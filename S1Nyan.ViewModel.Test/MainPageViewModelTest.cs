using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Moq;
using S1Nyan.Model;
using S1Nyan.Utils;
using S1Nyan.ViewModels;
using S1Parser;
using S1Parser.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S1Nyan.ViewModel.Test
{
    [TestClass]
    public class MainPageViewModelTest
    {
        private Mock<IDataService> _stubDataService;
        private Mock<IIndicator> _stubIndicator;
        private Mock<IErrorMsg> _stubErrorMsg;

        private string stubExceptionMsg = "Stub Exception Message";

        [TestInitialize]
        public void Setup()
        {
            _stubIndicator = new Mock<IIndicator>();
            _stubDataService = new Mock<IDataService>();
            _stubErrorMsg = new Mock<IErrorMsg>();
            _stubErrorMsg.Setup(x => x.GetExceptionMessage(It.IsAny<Exception>()))
                .Returns(stubExceptionMsg);
            Util.Indicator = _stubIndicator.Object;
            Util.ErrorMsg = _stubErrorMsg.Object;
        }

        [TestMethod]
        public async Task TestRefreshMainListData()
        {
            _stubDataService.Setup(x => x.UpdateMainListAsync())
                .Returns(() => Task<IList<S1ListItem>>.Factory.StartNew(
                    () => new List<S1ListItem>()));

            var mainPageViewModel = new MainPageViewModel(_stubDataService.Object, null, null, null);

            await mainPageViewModel.RefreshData();

            _stubIndicator.Verify(x => x.SetLoading());
            _stubDataService.Verify(x => x.GetMainListDone(true));
            _stubIndicator.Verify(x => x.SetBusy(false));
        }

        [TestMethod]
        public async Task TestRefreshMainListDataWithInvalidData()
        {
            _stubDataService.Setup(x => x.UpdateMainListAsync())
                .Throws(S1UserException.InvalidData);

            var mainPageViewModel = new MainPageViewModel(_stubDataService.Object, null, null, null);

            await mainPageViewModel.RefreshData();

            _stubIndicator.Verify(x => x.SetLoading());
            _stubDataService.Verify(x => x.GetMainListDone(false));
            _stubIndicator.Verify(x => x.SetError(It.IsAny<string>()));
        }
    }
}
