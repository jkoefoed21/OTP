using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stego_Stuff;
using encryption;
using System.Drawing;
using System.Security.Cryptography;

namespace OTPStuff
{
    static class OTPHandler
    {
        public static readonly int CONSTANT_MSG_LEN = (int) Math.Pow(2, 20); //fixed message Length of 1 Megabyte.

        /// <summary>
        /// The EOF character that is repeated a certain number times before EOF
        /// </summary>
        public static readonly byte EOF_CHAR1 = 0x01;

        /// <summary>
        /// The final character of the file
        /// </summary>
        public static readonly byte EOF_CHARFINAL = 0x04;

        /// <summary>
        /// The number of times the EOF1 char is repeated
        /// </summary>
        public static readonly byte EOF1_LENGTH = 16;

        public static byte[] generateRandomBytes(int length)
        {
            byte[] OTP = new byte[length];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(OTP);
            return OTP;
        }

        public static byte[] getEncryptedMessage(byte[] OTP, byte[] message) //OTP is symetrical--only differs in output length
        {
            byte[] output = new byte[CONSTANT_MSG_LEN];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(output);
            if (message.Length>OTP.Length)
            {
                throw new ArgumentException("OTP must be longer than message");
            }
            for (int ii=0; ii<message.Length; ii++)
            {
                output[ii] = (byte) (OTP[ii] ^ message[ii]);
            }
            return output;
        }

        public static byte[] extractEncryptedMessage(byte[] OTP, byte[] toDecrypt)
        {
            byte[] output;
            if (OTP.Length>toDecrypt.Length)
            {
                output = new byte[toDecrypt.Length];
            }
            else
            {
                output = new byte[OTP.Length];
            }
            for (int ii=0; ii<output.Length; ii++)
            {
                output[ii] = (byte) (toDecrypt[ii] ^ OTP[ii]);
            }
            return output;
        }

        /// <summary>
        /// Adds a EOF to existing byte array. Must allocate and copy array
        /// </summary>
        /// <param name="message"> The bytes without EOF</param>
        /// <returns> The bytes w/ EOF</returns>
        public static byte[] addEOF(byte[] message)
        {
            //Console.WriteLine(message.Length);
            byte[] bytesWEOF = new byte[message.Length + EOF1_LENGTH + 1 + 2 * AES.BLOCK_LENGTH];
            Array.Copy(message, bytesWEOF, message.Length);
            for (int ii = 0; ii < EOF1_LENGTH; ii++)//MAGIC
            {
                bytesWEOF[message.Length + ii] = EOF_CHAR1;
            }
            bytesWEOF[message.Length + EOF1_LENGTH] = EOF_CHARFINAL;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] r = new byte[AES.BLOCK_LENGTH * 2]; //adds 32 bytes of random data, but ensures that the EOF is not damaged by random data after the image.
            rng.GetBytes(r);
            Array.Copy(r, 0, bytesWEOF, message.Length + EOF1_LENGTH + 1, r.Length);
            return bytesWEOF;
        }

        public static byte[] chopEOF(byte[] message)
        {
            int endCount = 0;
            for (int ii = 0; ii < message.Length; ii++)
            {
                if (endCount >= EOF1_LENGTH && message[ii] == EOF_CHARFINAL)
                {
                    byte[] final = new byte[ii - EOF1_LENGTH];
                    Array.Copy(message, final, final.Length);
                    return final;
                }

                if (message[ii] == EOF_CHAR1)
                {
                    endCount++;
                }
                else
                {
                    endCount = 0;
                }
            }
            throw new ArgumentException("EOF not found");
        }
    }
}
