using System;
using System.Collections.Generic;
using System.Text;

using CoreTweet;

using System.Threading.Tasks;

namespace WordbookImpressApp.Storage
{
    public static class TwitterStorage
    {
        private static OAuth2Token appOnlyToken;

        public static async Task<OAuth2Token> GetAppOnlyToken()
        {
            if (appOnlyToken != null) return appOnlyToken;
            try
            {
                return appOnlyToken = await OAuth2.GetTokenAsync(APIKeys.TwitterConsumerKey, APIKeys.TwitterConsumerSecret);
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
