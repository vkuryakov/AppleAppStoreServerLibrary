using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace AppleAppStoreServerLibrary.Client
{
    public class BearerTokenAuthenticator
    {
        private static readonly string APP_STORE_CONNECT_AUDIENCE = "appstoreconnect-v1";
        private static readonly string BUNDLE_ID_KEY = "bid";

        private readonly byte[] _privateKey;
        private readonly string _keyId;
        private readonly string _issuerId;
        private readonly string _bundleId;

        public BearerTokenAuthenticator(string signingKey, string keyId, string issuerId, string bundleId)
        {
            signingKey = signingKey.Replace("-----BEGIN PRIVATE KEY-----", "")
                    .Replace("\\R+", "")
                    .Replace("-----END PRIVATE KEY-----", "");

            _privateKey = Convert.FromBase64String(signingKey);
            _keyId = keyId;
            _issuerId = issuerId;
            _bundleId = bundleId;
        }

        public string GenerateToken()
        {
            var expiresAt = DateTime.UtcNow.Add(TimeSpan.FromSeconds(1800));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = APP_STORE_CONNECT_AUDIENCE,
                Expires = expiresAt,
                Issuer = _issuerId,
                Claims = new Dictionary<string, object>
                {
                    {BUNDLE_ID_KEY, _bundleId }
                }
                
                //Subject = new ClaimsIdentity(new[] { new Claim("sub", "{YOUR CLIENT ID}") }),
            };

            using (var privateKey = CngKey.Import(_privateKey, CngKeyBlobFormat.Pkcs8PrivateBlob))
            {
                using (var algorithm = new ECDsaCng(privateKey))
                {
                    algorithm.HashAlgorithm = CngAlgorithm.Sha256;
                    var key = new ECDsaSecurityKey(algorithm) { KeyId = _keyId };

                    tokenDescriptor.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256Signature);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    return tokenHandler.CreateEncodedJwt(tokenDescriptor);
                }
            }
            //JwtHeader jwtHeader = new JwtHeader
            //{
            //    { "kid", _keyId },
            //    { "typ", "JWT" }
            //};
            //JwtPayload jwtPayload = new JwtPayload()
            //{
            //    {"Audience",APP_STORE_CONNECT_AUDIENCE },
            //    { "Expires", expiresAt },
            //    { }
            //};
            //JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(jwtHeader, jwtPayload);


            //return JWT.create()
            //        .withPayload(Map.of(BUNDLE_ID_KEY, bundleId))
            //        .sign(Algorithm.ECDSA256(_signingKey));
        }
    }
}
