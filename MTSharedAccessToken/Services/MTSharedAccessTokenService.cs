using Microsoft.IdentityModel.Tokens;
using MTSharedAccessToken.Helper;
using MTSharedAccessToken.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTSharedAccessToken.Services
{
    public class MTSharedAccessTokenService
    {
        public static string CreateSharedAccessToken(SharedAccessTokenSettings accessTokenSettings)
        {
            IMTAccessTokenHelper accessTokenHelper = new MTAccessTokenHelper();
            return accessTokenHelper.CreateSharedToken(accessTokenSettings);
        }

        public static TokenValidationParameters VerifySharedTokenSettings(SharedAccessTokenSettings accessTokenSettings)
        {
            IMTAccessTokenHelper accessTokenHelper = new MTAccessTokenHelper();
            return accessTokenHelper.VerifySharedToken(accessTokenSettings);
        }

        public static string GenerateUserAccessToken(SharedAccessTokenSettings accessTokenSettings,IList<string> assingRoles)
        {
            IMTAccessTokenHelper accessTokenHelper = new MTAccessTokenHelper();
            return accessTokenHelper.GenerateUserAccessToken(accessTokenSettings,assingRoles);
        }

    }
}
