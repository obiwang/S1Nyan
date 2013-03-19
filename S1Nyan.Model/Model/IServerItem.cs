using System;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.Model
{
    public interface IServerItem
    {
        IParserFactory ParserFactory { get; set; }
        string Addr { get; set; }
        S1UserException UserException { get; }
        void Cancel();
        void Check();
        Action<IServerItem> NotifyComplete { get; set; }
        Action<IServerItem> NotifySuccess { get; set; }
    }
}
