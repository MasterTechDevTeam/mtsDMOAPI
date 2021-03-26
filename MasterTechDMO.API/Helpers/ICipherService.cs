using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Helpers
{
    public interface ICipherService
    {
        string Encrypt(string input, string key);

        string Decrypt(string cipherText, string key);
    }
}
