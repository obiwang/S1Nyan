using System;
using System.Collections.Generic;
using S1Parser;

namespace S1Nyan.Model
{
    public interface IServerItem
    {
        IParserFactory ParserFactory { get; set; }
        string Addr { get; set; }
        Exception UserException { get; }
        void Cancel();
        void Check();
        Action<IServerItem> NotifyComplete { get; set; }
        Action<IServerItem> NotifySuccess { get; set; }
    }
}
