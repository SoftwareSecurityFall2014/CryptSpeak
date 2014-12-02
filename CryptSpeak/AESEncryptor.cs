using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptSpeak
{
    public class AESEncryptor : Encryptor 
    {
        byte[] key;

        public AESEncryptor(byte[] key)
        {
            this.key = key;
        }
        public override byte[] Encrypt(byte[] mes)
        {
            byte[] ret = mes;

            return ret;
        }


        public override byte[] Decrypt(byte[] mes)
        {
            byte[] ret = mes;

            return ret;
        }

        public override void SetKey(byte[] key)
        {
            return;
        }
    }
}
