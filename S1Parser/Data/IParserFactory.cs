﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S1Parser
{
    public interface IParserFactory
    {
        IResourceService ResourceService { get; set; }
        Task<IList<S1ListItem>> GetMainListData();
        Task<Stream> GetMainListStream();
        IList<S1ListItem> ParseMainListData(string s);
        IList<S1ListItem> ParseMainListData(Stream s);

        Task<S1ThreadList> GetThreadListData(string fid, int page);
        Task<S1Post> GetPostData(string tid, int page);
        string Path { get; }
        string GetThreadOriginalUrl(string tid);
    }
}
