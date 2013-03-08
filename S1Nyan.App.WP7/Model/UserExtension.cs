using System;
using System.Linq;
using System.Threading.Tasks;
using S1Parser;

namespace S1Nyan.Model
{

    public static class UserExtension
    {
        public static async Task<string> GetVerifyString(this S1WebClient client)
        {
            string verify = "";
            try
            {
                var privacyPage = await client.DownloadStringTaskAsync(new Uri(S1Parser.Action.UserAction.PrivacyUrl));
                var doc = new HtmlDoc(privacyPage).RootElement;
                var inputs = from e in doc.FindElements("input")
                             where e.Attributes["name"] == "verify"
                             select e;
                verify = inputs.First().Attributes["value"];
            }
            catch
            {

            }
            return verify;
        }
    }
}
