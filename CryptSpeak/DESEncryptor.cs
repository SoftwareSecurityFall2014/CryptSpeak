using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptSpeak.Encryption
{
    public class DESEncryptor
    {
        //Oficially using MSB as standard
        /*      TODOS:
         *          Take in a string and convert to a encrypted byte array
         *          Take in a cypher string and convert it back to a string
         *          Key needs to be 64 bit, select 56 bits, split into two
         *              28 bit segments and rotate let "one or two bits"
         *              each round
        */

        //Keys are 8 bit
        public byte[] keySchedule;
        //For a rotation schedule to work, they must
        //  A. The rotations must add up to 28 for a 64 bit key
        //      (In generating the key used, the last byte of the key is ignored, left with 56 bits)
        //      (This is split in half, to 28 bit halves)
        //      (Each round of the cypher, each half is circularly shifted to the left (*2))
        //      (The first 24 bits of each are added together, ignoring the 4 bits on each end
        //      (This leaves you with a 48 bit key, which is xor'd with the eselection of the message)
        //IMPORTANT:    If any of the rotation schedule values are > 4, you will need to edit the key generating
        //              function to move the nibbles (shiftval/4 to the left) and then shift bits (shiftval%4)
        //              Not emplemented yet (its a pain...)
        public int[] rotationSchedule = {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1};
        //This will be a [16] [6] 2dim byte array
        //[16] is the round number the key is used in
        //[6] is th number of bytes in the key (48 bits)
        public byte[][] pregeneratedKeys;
        //This is a suggested sbox from NIST
        public byte[,] sbox = {
                                {0x0E, 0x04, 0x0D, 0x01, 0x02, 0x0F, 0x0B, 0x08, 0x03, 0x0A, 0x06, 0x0C, 0x05, 0x09, 0x00, 0x07},
                                {0x00, 0x0F, 0x07, 0x04, 0x0E, 0x02, 0x0D, 0x01, 0x0A, 0x06, 0x0C, 0x0B, 0x09, 0x05, 0x03, 0x08},
                                {0x04, 0x01, 0x0E, 0x08, 0x0D, 0x06, 0x02, 0x0B, 0x0F, 0x0C, 0x09, 0x07, 0x03, 0x0A, 0x05, 0x00},
                                {0x0F, 0x0C, 0x08, 0x02, 0x04, 0x09, 0x01, 0x07, 0x05, 0x0B, 0x03, 0x0E, 0x0A, 0x00, 0x06, 0x0D}
                              };

        public DESEncryptor(string keyFile)
        {
            string textinfo = System.IO.File.ReadAllText(@keyFile);

            keySchedule = new byte[8];
            pregeneratedKeys = new byte[16][];

            for (int i = 0; i < 16; i +=2)
            {
                byte val = (byte)((conCharToInt(textinfo[i]) << 4) + conCharToInt(textinfo[i + 1]));
                keySchedule[i/2] = val;
            }

            Console.Out.WriteLine(textinfo);

            GenerateKeyCycleValues();
            //keySchedule = new byte[8];

            //Random rdm = new Random();

            //rdm.NextBytes(keySchedule);
        }

        public byte[] encryptMessage(string mes)
        {
            ASCIIEncoding enc = new ASCIIEncoding();

            //Check if does not end evenly mesbytes.Length%8 != 0
            //Add extra somehow (look at CSC-580 slides for method)
            //Only instance where this would happen would be
            //      mes.Length % 4 != 0
            //Possible to just add extra spaces. I'll do that for now
            if (mes.Length % 8 != 0)
            {
                for (int i = 0; i < (mes.Length % 8); i++)
                {
                    mes += " ";
                }
            }
            byte[] mesbytes = enc.GetBytes(mes);
            byte[] outByte = new byte[mesbytes.Length];

            for (int i = 0; i < (mesbytes.Length/8); i++)
            {
                byte[] block = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    block[j] = mesbytes[j + (8 * i)];
                }

                block = InitPerm(block);

                for (int j = 0; j < 16; j++)
                {
                    block = roundDES(block, j);
                }
                block = SwapLR(block);

                block = InvInitPerm(block);
                for (int k = 0; k < 8; k++)
                {
                    outByte[k + (i * 8)] = block[k];
                }
            }

            return outByte;
        }


        public string decryptMessage(byte[] mes)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            string outStr = "";

            byte[] mesbytes = mes;

            for (int i = 0; i < (mesbytes.Length / 8); i++)
            {
                byte[] block = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    block[j] = mesbytes[j + (8 * i)];
                }

                block = InitPerm(block);

                for (int j = 15; j >= 0; j--)
                {
                    block = roundDES(block, j);
                }
                block = SwapLR(block);

                block = InvInitPerm(block);

                outStr += enc.GetString(block);
            }

            return outStr;
        }

        public byte[] roundDES(byte[] block, int keynum)
        {
            byte[] ret = new byte[8];
            byte[] lhs = new byte[4];
            byte[] rhs = new byte[4];

            for (int k = 0; k < 4; k++)
            {
                lhs[k] = block[k];
                rhs[k] = block[k + 4];
            }

            for (int k = 0; k < 4; k++)
            {
                ret[k] = rhs[k];
            }

            rhs = XOR(lhs, functionDES(rhs, keynum));

            for (int k = 0; k < 4; k++)
            {
                ret[k + 4] = rhs[k];
            }

            return ret;
        }

        public byte[] functionDES(byte[] mes, int keynum)
        {
            byte[] ret = ESelection(mes);
            //The pregenerated key list actually has 56 byte keys
            //For now we will ignore the last 4 bits in each 28 bit half,
            //so that we can get a 48 bit total instead
            //THE ACTUAL KEY FOR THE ROUND
            byte[] calKey = new byte[6];

            calKey[0] = pregeneratedKeys[keynum][0];
            calKey[1] = pregeneratedKeys[keynum][1];
            //Cutting off the last 4 bytes from the first half, adding the first 4 bytes of the second half (to calKey)
            calKey[2] = (byte)((pregeneratedKeys[keynum][2] & 0xF0) | (pregeneratedKeys[keynum][3] >> 4));
            calKey[3] = (byte)((pregeneratedKeys[keynum][3] << 4) | (pregeneratedKeys[keynum][4] >> 4));
            //This also cuts off the last 4 bits from the second half
            calKey[4] = (byte)((pregeneratedKeys[keynum][4] << 4) | (pregeneratedKeys[keynum][5] >> 4));

            //Uses calKey
            ret = XOR(ret, calKey);
            ret = sboxReplace(ret);
            return roundPermutation(ret);
        }

        public byte[] roundPermutation(byte[] mes)
        {
            byte[] ret = new byte[4];
            //(0000 0000)
            ret[0] = (byte)(((mes[1] & 0x01) << 7) |
                ((mes[0] & 0x02) << 5) |
                ((mes[2] & 0x10) << 1) |
                ((mes[2] & 0x08) << 1) |
                ((mes[3] & 0x08)) |
                ((mes[1] & 0x10) >> 2) |
                ((mes[3] & 0x10) >> 3) |
                ((mes[2] & 0x80) >> 7));
            ret[1] = (byte)(((mes[0] & 0x80)) |
                ((mes[1] & 0x01) << 6) |
                ((mes[2] & 0x02) << 4) |
                ((mes[3] & 0x40) >> 2) |
                ((mes[0] & 0x08)) |
                ((mes[2] & 0x40) >> 4) |
                ((mes[3] & 0x02)) |
                ((mes[1] & 0x40) >> 6));
            ret[2] = (byte)(((mes[0] & 0x40) << 1) |
                ((mes[0] & 0x01) << 6) |
                ((mes[2] & 0x01) << 5) |
                ((mes[1] & 0x04) << 2) |
                ((mes[3] & 0x01) << 3) |
                ((mes[3] & 0x20) >> 3) |
                ((mes[0] & 0x20) >> 4) |
                ((mes[1] & 0x80) >> 7));
            ret[3] = (byte)(((mes[2] & 0x20) << 6)|
                ((mes[1] & 0x08) << 3)|
                ((mes[3] & 0x04) << 3)|
                ((mes[0] & 0x04) << 2)|
                ((mes[2] & 0x04) << 1)|
                ((mes[1] & 0x20) >> 3)|
                ((mes[0] & 0x10) >> 3)|
                ((mes[3] & 0x80) >> 7));

            return ret;
        }

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

        public void GenerateKeyCycleValues()
        {
            //Only use 56 bits
            //Only cycle 28 bits
            //Only concat 24 bits
            pregeneratedKeys[0] = RotateHalves(keySchedule, rotationSchedule[0]);
            //This will generate the key schedule backwards
            for (int i = 1; i < rotationSchedule.Length; i++)
            {
                pregeneratedKeys[i] = RotateHalves(pregeneratedKeys[i-1], rotationSchedule[i]);
            }
            //End result will be array of 6 bytes (48 bits)
        }

        public byte[] RotateHalves(byte[] mes, int shift)
        {
            //  Currently if a value greater than 4 is in the rotation schedule
            //      this rotation will break. It can be fixed, its just a pain.
            byte[] ret = new byte[7];
            byte[] half1 = new byte[4];
            byte[] half2 = new byte[4];

            for (int i = 0; i < 3; i++)
            {
                half1[i] = mes[i];
                half2[i+1] = mes[i + 4];
            }
            half1[3] = (byte)(0xF0 & mes[3]);
            half2[0] = (byte)(0x0F & mes[3]);

            byte toNext = 0x00;
            byte fromLast = 0x00;
            //half1[3] = half1[3] << shift | half1[3] >> (8-shift);
            //fromLast = toNext;
            //toNext = half1[3] & (0xFF >> (8 - shift));
            //half1[3] = half1[3] & (0xFF << shift);
            //half1[3] = half1[3] & fromLast
            for (int i = 3; i >= 0; i--)
            {
                half1[i] = (byte)(half1[i] << shift | half1[i] >> (8 - shift));
                fromLast = toNext;
                toNext = (byte)(half1[i] & (0xFF >> (8 - shift)));
                half1[i] = (byte)(half1[i] & (0xFF << shift));
                half1[i] = (byte)(half1[i] | fromLast);
            }
            half1[3] = (byte)(half1[3] | (toNext << 4));

            fromLast = 0x00;
            toNext = 0x00;
            for (int i = 3; i >= 0; i--)
            {
                half2[i] = (byte)(half2[i] << shift | half2[i] >> (8 - shift));
                fromLast = toNext;
                toNext = (byte)(half2[i] & (0xFF >> (8 - shift)));
                half2[i] = (byte)(half2[i] & (0xFF << shift));
                half2[i] = (byte)(half2[i] | fromLast);
            }
            toNext = (byte)(half2[0] & 0xF0);
            half2[0] = (byte)(half2[0] & 0x0F);
            half2[3] = (byte)(half2[3] | (toNext >> 4));

            for (int i = 0; i < 3; i++)
            {
                ret[i] = half1[i];
            }
            ret[3] = (byte)(half1[3] | half2[0]);
            for (int i = 4; i < 7; i++)
            {
                ret[i] = half2[i-3];
            }

            return ret;
        }

        public byte[] InitPerm(byte[] mes)
        {
            byte[] ret = new byte[8];
            int counter = 0;

            for (int i = 57; i < 64; i += 2)
            {
                for (int j = 0; j < 64; j += 8)
                {
                    int locByt = (i - j) / 8;
                    int locBit = 7 - ((i - j)%8);

                    if ((mes[locByt] & (1 << locBit)) != 0)
                    {
                        ret[(counter / 8)] += (byte)(1 << (7 - (counter % 8)));
                    }
                    counter++;
                }
            }
            for (int i = 56; i < 63; i += 2)
            {
                for (int j = 0; j < 64; j += 8)
                {
                    int locByt = (i - j) / 8;
                    int locBit = 7 - ((i - j) % 8);

                    if ((mes[locByt] & (1 << locBit)) != 0)
                    {
                        ret[(counter / 8)] += (byte)(1 << (7 - (counter % 8)));
                    }
                    counter++;
                }
            }

            return ret;
        }

        public byte[] InvInitPerm(byte[] mes)
        {
            byte[] ret = new byte[8];
            int counter = 0;

            for (int i = 39; i >= 32; i--)
            {
                for (int j = 0; j < 32; j += 8)
                {
                    int locByt = (i + j) / 8;
                    int locBit = 7 - ((i + j) % 8);

                    if ((mes[locByt] & (1 << locBit)) != 0)
                    {
                        ret[(counter / 8)] += (byte)(1 << (7 - (counter % 8)));
                    }
                    counter += 2;
                }
            }
            counter = 1;
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 32; j += 8)
                {
                    int locByt = (i + j) / 8;
                    int locBit = 7 - ((i + j) % 8);

                    if ((mes[locByt] & (1 << locBit)) != 0)
                    {
                        ret[(counter / 8)] += (byte)(1 << (7 - (counter % 8)));
                    }
                    counter += 2;
                }
            }

            return ret;
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

        public byte[] SwapLR(byte[] mes)
        {
            byte[] ret = new byte[mes.Length];
            for (int i = 0; i < (mes.Length / 2); i++)
            {
                ret[i] = mes[i + (mes.Length / 2)];
                ret[i + (mes.Length / 2)] = mes[i];
            }
            return ret;
        }

        /*
            *   Will return 48 bit Eselection from 32 bit message in
            *   this format (every number is a bit in the message)
            *   48 bit ESelection:
            *       31 00 01 02 03 04
            *       03 04 05 06 07 08
            *       07 08 09 10 11 12
            *       11 12 13 14 15 16
            *       15 16 17 18 19 20
            *       19 20 21 22 23 24
            *       23 24 25 26 27 28
            *       27 28 29 30 31 00
        */
        public byte[] ESelection(byte[] mes)
        {
            byte[] ret = new byte[6];

            int counter = 0;

            // i = (2^8 - 1); i < (2^8 * 2) - 1; i += 4
            // Reasoning:   The first of every 6 bits is the (2^(rowofsixbits) - 1)%32 bit of the message
            //              Each row increases by 4
            //              Then the other 5 bits are the next 5 bits in the message
            for (byte i = 31; i < 63; i += 4)
            {
                for (byte j = 0; j < 6; j++)
                {
                    int locByt = ((i + j)%32)/8;
                    int locBit = 7 - ((i + j)%8);
                    if ((mes[locByt] & (1 << locBit)) != 0)
                    {
                        ret[(counter/8)] += (byte)(1 << (7-(counter%8)));
                    }
                    counter++;
                }
            }

            return ret;
        }

        //Will take in a 48 bit message [6]
        //Spits out a 32 bit replacement [4]
        public byte[] sboxReplace(byte[] mes)
        {
            //Each 6 bit segment of mes will be replaced with a 4 bit segment
            //  Looked up in the sbox
            //EX:   110010
            //      The row is [1] 1001 [0] (the outer bits) = 10 = (dec) 2
            //      The column is 1 [1001] 0 (inner bits) = 1001 = (dec) 9
            byte[] ret = new byte[4];

            // {[000000] 00} {00000000} {00000000}
            byte row = (byte)(((mes[0] & 0x80) >> 6) | ((mes[0] & 0x04) >> 2));
            byte col = (byte)(((mes[0] & 0x70) >> 3) | ((mes[0] & 0x08) >> 3));
            ret[0] = (byte)(sbox[row, col] << 4);

            // {000000 [00]} {[0000] 0000} {00000000}
            row = (byte)(((mes[0] & 0x02)) | ((mes[1] & 0x10) >> 4));
            col = (byte)(((mes[0] & 0x01) << 3) | ((mes[1] & 0xE0) >> 5));
            ret[0] = (byte)(ret[0] | sbox[row, col]);

            // {00000000} {0000 [0000]} {[00] 000000}
            row = (byte)(((mes[1] & 0x08) >> 2) | ((mes[2] & 0x40) >> 6));
            col = (byte)(((mes[1] & 0x07) << 1) | ((mes[2] & 0x80) >> 7));
            ret[1] = (byte)(sbox[row, col] << 4);

            // {00000000} {00000000} {00 [000000]}
            row = (byte)(((mes[2] & 0x20) >> 4) | ((mes[2] & 0x01)));
            col = (byte)((mes[2] & 0x1E) >> 1);
            ret[1] = (byte)(ret[1] | sbox[row, col]);



            // {[000000] 00} {00000000} {00000000}
            row = (byte)(((mes[3] & 0x80) >> 6) | ((mes[3] & 0x04) >> 2));
            col = (byte)(((mes[3] & 0x70) >> 3) | ((mes[3] & 0x08) >> 3));
            ret[2] = (byte)(sbox[row, col] << 4);

            // {000000 [00]} {[0000] 0000} {00000000}
            row = (byte)(((mes[3] & 0x02)) | ((mes[4] & 0x10) >> 4));
            col = (byte)(((mes[3] & 0x01) << 3) | ((mes[4] & 0xE0) >> 5));
            ret[2] = (byte)(ret[2] | sbox[row, col]);

            // {00000000} {0000 [0000]} {[00] 000000}
            row = (byte)(((mes[4] & 0x08) >> 2) | ((mes[5] & 0x40) >> 6));
            col = (byte)(((mes[4] & 0x07) << 1) | ((mes[5] & 0x80) >> 7));
            ret[3] = (byte)(sbox[row, col] << 4);

            // {00000000} {00000000} {00 [000000]}
            row = (byte)(((mes[5] & 0x20) >> 4) | ((mes[5] & 0x01)));
            col = (byte)((mes[5] & 0x1E) >> 1);
            ret[3] = (byte)(ret[3] | sbox[row, col]);

            return ret;
        }
    }
}
