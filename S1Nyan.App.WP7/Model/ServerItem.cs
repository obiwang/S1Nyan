using System;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.Model
{
    public class ServerItem : IServerItem
    {
        public string Addr { get; set; }

        public Action<IServerItem> NotifySuccess { get; set; }

        public Action<IServerItem> NotifyComplete { get; set; }

        public IParserFactory ParserFactory { get; set; }

        S1WebClient client = null;

        public Exception UserException { get; private set; }

        public async void Check()
        {
            try
            {
                client = new S1WebClient();
                var result = await client.DownloadStringTaskAsync(Addr + ParserFactory.Path);
                if (result.Length > 0)
                {
                    var data = ParserFactory.ParseMainListData(result);
                    if (NotifySuccess != null /*&& data.Count > 0*/)
                    {
                        System.Diagnostics.Debug.WriteLine("NotifySuccess " + Addr);
                        NotifySuccess(this);
                        return;
                    }
                }
            }
            catch (S1UserException e) { 
                if(e.Message != null ) 
                    UserException = e; 
            }
            catch (Exception) { }
            finally
            {
                if (NotifyComplete != null)
                    NotifyComplete(this);
            }
        }

        public void Cancel()
        {
            if (client != null && client.IsBusy)
            {
                client.CancelAsync();
            }
        }
    }
}
