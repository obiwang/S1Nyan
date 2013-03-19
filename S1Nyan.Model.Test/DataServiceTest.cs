using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using S1Parser;
using S1Nyan.Model;
using S1Parser.SimpleParser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S1Nyan.Model.Test
{
    [TestClass]
    public class DataServiceTest
    {
        IParserFactory stubPaserFactory;
        MockRepository mocks;
        
        [TestInitialize()]
        public void Setup()
        {
            mocks = new MockRepository();
            stubPaserFactory = mocks.Stub<IParserFactory>();
        }

        //[TestMethod]
        //public async void TestGetMainListData()
        //{
        //    using (mocks.Record())
        //    {
        //        await stubPaserFactory.GetMainListData();
        //        LastCall.Return(new List<S1ListItem>());
        //    }

        //    DataService service = new DataService { ParserFactory = stubPaserFactory };
        //    //var result = await service.GetMainListAsync();

        //    //await Dela.AsyncAsserts.ThrowsExceptionAsync<Exception>(service.GetMainListAsync);
        //    //Assert.IsTrue(result is List<S1ListItem>);
        //}

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
