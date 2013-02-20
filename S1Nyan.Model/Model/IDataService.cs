using System;
using System.Collections.Generic;
using S1Parser;

namespace S1Nyan.Model
{
    public interface IDataService
    {
        void GetMainListData(Action<List<S1ListItem>, Exception> callback);
        void GetThreadListData(string fid, int page, Action<S1ThreadList, Exception> callback);
        void GetThreadData(string tid, int page, Action<S1ThreadPage, Exception> callback);
        IResourceService ResourceService { get; set; }
    }
}
