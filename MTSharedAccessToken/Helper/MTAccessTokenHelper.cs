using Microsoft.IdentityModel.Tokens;
using MTSharedAccessToken.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MTSharedAccessToken.Helper
{
    public class MTAccessTokenHelper : IMTAccessTokenHelper
    {
        public string CreateSharedToken(SharedAccessTokenSettings accessTokenSettings)
        {
            try
            {
                var now = DateTime.Now;
                var claims = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,accessTokenSettings.SharedName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                var encKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(accessTokenSettings.SharedKey));

                var tokenValidationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = encKey,
                    ValidateIssuer = true,
                    ValidIssuer = accessTokenSettings.IssuerName,
                    ValidateAudience = true,
                    ValidAudience = accessTokenSettings.AudienceName,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };

                var jwtSecToken = new JwtSecurityToken(
                    issuer: accessTokenSettings.IssuerName,
                    audience: accessTokenSettings.AudienceName,
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(accessTokenSettings.ExpTimeInMinute)),
                    signingCredentials: new SigningCredentials(encKey, SecurityAlgorithms.HmacSha256)
               );

                var token = new JwtSecurityTokenHandler().WriteToken(jwtSecToken);

                UserAccessToken accessToken = new UserAccessToken
                {
                    Token = token,
                    ExpIn = (int)TimeSpan.FromMinutes(accessTokenSettings.ExpTimeInMinute).TotalSeconds
                };

                return JsonConvert.SerializeObject(accessToken);
            }
            catch (Exception Ex)
            {

                return string.Empty;
            }
        }

        public string GenerateUserAccessToken(SharedAccessTokenSettings accessTokenSettings, IList<string> assignRoles)
        {
            try
            {
                var encKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(accessTokenSettings.SharedKey));
                var cred = new SigningCredentials(encKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                foreach(var role in assignRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role,role));
                }

                var token = new JwtSecurityToken(accessTokenSettings.IssuerName,
                    accessTokenSettings.AudienceName,
                    claims,
                    expires: DateTime.Now.AddMinutes(accessTokenSettings.ExpTimeInMinute),
                    signingCredentials: cred);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception Ex)
            {

                return string.Empty;
            }
        }

        public TokenValidationParameters VerifySharedToken(SharedAccessTokenSettings accessTokenSettings)
        {
            try
            {
                var encKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(accessTokenSettings.SharedKey));

                return new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = encKey,
                    ValidateIssuer = true,
                    ValidIssuer = accessTokenSettings.IssuerName,
                    ValidateAudience = true,
                    ValidAudience = accessTokenSettings.AudienceName,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };
            }
            catch (Exception Ex)
            {

                return null;
            }
        }
    }
}
