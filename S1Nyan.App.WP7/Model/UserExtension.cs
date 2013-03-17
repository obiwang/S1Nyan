using System;
using System.Threading.Tasks;
using S1Parser;
using S1Parser.User;

namespace S1Nyan.Model
{

    public static class UserExtension
    {
        public static async Task<string> GetVerifyString(this S1WebClient client)
        {
            string verify = "";
            
            //use DownloadString will just return cached data, which is not what i want
            //post dummy data to disable cache
            var privacyPage = await client.PostDataTaskAsync(new Uri(UserAction.PrivacyUrl)); 
            var root = new HtmlDoc(privacyPage).RootElement;
            var input = root.FindFirst("input", (e) => e.Attributes["name"] == "verify");
            if (input != null)
                verify = input.Attributes["value"];
            else
            {   
                throw new S1UserException(UserErrorTypes.Unknown);
            }
            return verify;
        }
    }
}
