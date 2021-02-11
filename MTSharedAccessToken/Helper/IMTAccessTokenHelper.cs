using Microsoft.IdentityModel.Tokens;
using MTSharedAccessToken.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTSharedAccessToken.Helper
{
    public interface IMTAccessTokenHelper
    {
        string CreateSharedToken(SharedAccessTokenSettings accessTokenSettings);

        TokenValidationParameters VerifySharedToken(SharedAccessTokenSettings accessTokenSettings);

        string GenerateUserAccessToken(SharedAccessTokenSettings accessTokenSettings, IList<string> assignRoles);
    }
}
