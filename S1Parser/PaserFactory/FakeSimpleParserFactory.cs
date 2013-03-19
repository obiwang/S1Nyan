using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using S1Parser.SimpleParser;

namespace S1Parser.PaserFactory
{
    public class FakeSimpleParserFactory : SimpleParserFactory
    {
        protected override Uri GetMainUri()
        {
            return new Uri("FakeData/simple.htm", UriKind.Relative);
        }

        protected override Uri GetThreadListUri(string fid, int page)
        {
            return new Uri("FakeData/simple_thread.htm", UriKind.Relative);
        }

        protected override Uri GetThreadUri(string tid, int page)
        {
            return new Uri("FakeData/simple_read.htm", UriKind.Relative);
        }
    }
}
