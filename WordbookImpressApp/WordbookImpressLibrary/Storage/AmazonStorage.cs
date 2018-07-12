using System;
using System.Collections.Generic;
using System.Text;

using Nager.AmazonProductAdvertising;

namespace WordbookImpressLibrary.Storage
{
    public static class AmazonStorage
    {
        private static AmazonWrapper amazonWrapper;
        public static AmazonWrapper AmazonWrapper => amazonWrapper = amazonWrapper ?? GetAmazonWrapper(APIKeys.AmazonAssociateTag);

        private static AmazonWrapper amazonWrapperShare;
        public static AmazonWrapper AmazonWrapperShare => amazonWrapperShare = amazonWrapperShare ?? GetAmazonWrapper(APIKeys.AmazonAssociateTagShare);

        public static AmazonWrapper GetAmazonWrapper(string tag)
        {
            var authentification = new AmazonAuthentication();
            authentification.AccessKey = APIKeys.AmazonAccessKey;
            authentification.SecretKey = APIKeys.AmazonSecretKey;
            return new AmazonWrapper(authentification, AmazonEndpoint.JP, tag);
        }
    }
}
