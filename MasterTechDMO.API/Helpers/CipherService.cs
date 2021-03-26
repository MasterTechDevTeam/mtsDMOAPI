using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Helpers
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public CipherService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input,string key)
        {
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText,string key)
        {
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Unprotect(cipherText);
        }
    }
}
