using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptSpeak
{
    public abstract class Encryptor
    {
        public abstract byte[] Encrypt(byte[] mes);
        public abstract byte[] Decrypt(byte[] mes);
        public abstract void SetKey(byte[] key);
    }
}
