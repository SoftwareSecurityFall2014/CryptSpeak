using CryptSpeak.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptSpeak
{
    public class EncryptionManager
    {
        Encryptor encMethod;
        int strat;
        byte[] lastSend;
        byte[] lastReceive;

        public enum EncType : byte { None, DES, AES, Elliptic };
        public enum EncMeth : byte { ECB, CBC, CFB, OFB, CTR };

        public EncryptionManager(string keyLoc, string nunce, byte type, byte method)
        {
            switch(type)
            {
                case (int)EncType.DES:
                    encMethod = new DESEncryptor(GetKey(keyLoc));
                    break;
                case (int)EncType.AES:
                    break;
                case (int)EncType.Elliptic:
                    break;
            }
            strat = method;
            if(method != (int)EncMeth.ECB)
            {
                lastReceive = GetNunce(nunce);
                lastSend = GetNunce(nunce);
                
            }
        }

        public byte[] Encrypt(string mes)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            //if (mes.Length % 8 != 0)
            //{
            //    for (int i = 0; i < (mes.Length % 8); i++)
            //    {
            //        mes += " ";
            //    }
            //}
            byte[] fullMess = enc.GetBytes(mes);
            byte[] ret = new byte[fullMess.Length];
            for (int i = 0; i < fullMess.Length; i += 8)
            {
                byte[] toEnc = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    toEnc[j] = fullMess[i + j];
                }
                byte[] toAdd;
                switch (strat)
                {
                    case (int)EncMeth.ECB:
                        toAdd = encMethod.Encrypt(toEnc);
                        for (int j = 0; j < 8; j++)
                        {
                            ret[j + i] = toAdd[j];
                        }
                        break;
                    case (int)EncMeth.CBC:
                        //XOR last plaintext with last sent cyphertext (or nunce)
                        //Encrypt
                        toAdd = CBCEncrypt(toEnc);
                        for(int j = 0; j < 8; j++)
                        {
                            ret[j+i] = toAdd[j];
                        }
                        break;
                    case (int)EncMeth.CFB:
                        ret = encMethod.Encrypt(toEnc);
                        break;
                    case (int)EncMeth.OFB:
                        ret = encMethod.Encrypt(toEnc);
                        break;
                    case (int)EncMeth.CTR:
                        ret = encMethod.Encrypt(toEnc);
                        break;
                }
            }
            return ret;
        }

        public string Decrypt(byte[] mes)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] ret = null;
            switch (strat)
            {
                case (int)EncMeth.ECB:
                    ret = encMethod.Decrypt(mes);
                    break;
                case (int)EncMeth.CBC:
                    //Decrypt
                    //Take last received cyphered text (or nunce if first time)
                    //XOR with Decrypt
                    ret = CBCDecrypt(mes);
                    break;
                case (int)EncMeth.CFB:
                    ret = encMethod.Decrypt(mes);
                    break;
                case (int)EncMeth.OFB:
                    ret = encMethod.Decrypt(mes);
                    break;
                case (int)EncMeth.CTR:
                    ret = encMethod.Decrypt(mes);
                    break;
            }
            return enc.GetString(ret);
        }

        public byte[] CBCEncrypt(byte[] toEnc)
        {
            byte[] ret = encMethod.Encrypt(XOR(lastSend, toEnc));
            for (int i = 0; i < lastSend.Length; i++)
            {
                lastSend[i] = ret[i];
            }
            return ret;
        }

        public byte[] CBCDecrypt(byte[] mes)
        {
            byte[] ret = XOR(lastReceive, encMethod.Decrypt(mes));
            for (int i = 0; i < lastReceive.Length; i++)
            {
                ;'

                lastReceive[i] = mes[i];
            }
            return ret;
        }

        public byte[] GetKey(string keyFile)
        {
            string textinfo = System.IO.File.ReadAllText(@keyFile);
            byte[] keySchedule = new byte[8];

            for (int i = 0; i < 16; i += 2)
            {
                //Using 16 and 2 for a reason (check textinfo[i])
                byte val = (byte)((conCharToInt(textinfo[i]) << 4) + conCharToInt(textinfo[i + 1]));
                keySchedule[i / 2] = val;
            }

            return keySchedule;
        }

        public byte[] GetNunce(string nunceFile)
        {
            string textinfo = System.IO.File.ReadAllText(@nunceFile);
            byte[] nunce = new byte[8];

            for (int i = 0; i < 16; i += 2)
            {
                //Using 16 and 2 for a reason (check textinfo[i])
                byte val = (byte)((conCharToInt(textinfo[i]) << 4) + conCharToInt(textinfo[i + 1]));
                nunce[i / 2] = val;
            }

            return nunce;
        }

        public byte[] XOR(byte[] mes1, byte[] mes2)
        {
            byte[] ret = new byte[mes1.Length];
            for (int i = 0; i < mes1.Length; i++)
            {
                ret[i] = (byte)(mes1[i] ^ mes2[i]);
            }
            return ret;
        }

        //For reading the text file chars as hex
        public int conCharToInt(char a)
        {
            if (a >= '0' && a <= '9')
            {
                return (int)a - 48;
            }
            if (a >= 'a' && a <= 'z')
            {
                return (int)a - 87;
            }

            if (a >= 'A' && a <= 'Z')
            {
                return (int)a - 55;
            }
            return -1;
        }
    }
}
