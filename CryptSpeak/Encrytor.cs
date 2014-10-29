using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptSpeak
{
    public abstract class Encrytor
    {
        public abstract byte[] Encrypt(string mes);
        public abstract string Decrypt(byte[] mes);
    }
}
