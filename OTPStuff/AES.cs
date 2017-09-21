using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;

namespace encryption 
{
    /// <summary>
    /// Handles overall program execution.
    /// </summary>
    static class AES
    {
        /// <summary>
        /// The number of rounds for 128 bit AES
        /// </summary>
        public static readonly int ROUNDS = 11; //this is 11 because I need 11 round keys for 10 ROUNDS+an initial RK

        /// <summary>
        /// The GF(2^8) table used throughout the program.
        /// </summary>
        public static readonly GF28Table GF_TABLE = new GF28Table();

        /// <summary>
        /// The s-box used throughout the table.
        /// </summary>
        public static readonly sBox SUB_TABLE = new sBox(GF_TABLE);

        /// <summary>
        /// The Length of the salt.
        /// </summary>
        public static readonly int SALT_LENGTH = 64; //bytes not bits

        /// <summary>
        /// The length of 128 bits in bytes
        /// </summary>
        public static readonly int BLOCK_LENGTH = 16; //bytes not bits

        /// <summary>
        /// The length of the password hash.
        /// </summary>
        public static readonly int HASH_LENGTH = 64; //bytes

        /// <summary>
        /// The length of salt, iv, and hash combined
        /// </summary>
        public static readonly int START_LENGTH = SALT_LENGTH + BLOCK_LENGTH + HASH_LENGTH;

        /// <summary>
        /// The length of a word in bytes
        /// </summary>
        public static readonly int WORD_LENGTH = 4;

        /// <summary>
        /// The Number of iterations used for the PBKDF2. This slows the program down a lot
        /// but it is good that it does, because it makes the hash, iv cryptographically secure.
        /// </summary>
        public static readonly int NUM_ITERATIONS = 30000; //slows the algorithm down by about a second...for security though

        /*
        ///<summary>
        ///the entry point for the program. Opens the UI.
        ///</summary>
        ///<param name="args"> Command line input</param>
        [STAThread] //This is needed for windows forms.
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Rijndael());
        }*/

        /// <summary>
        /// Encrypts on a file basis--replaces the old encryption main
        /// </summary>
        /// <param name="password">The password to be used</param>
        /// <param name="filePath">The file to be encrypted</param>
        public static void encryptionFromFile(String password, String filePath)
        {
            byte[] readBytes = File.ReadAllBytes(filePath);
            byte[] output=encryptionMain(password, readBytes);
            File.WriteAllBytes(filePath, output);
        }

        ///<summary>
        ///Manages the encryption process.
        ///</summary>
        ///<param name="password"> Password to be encrypted with </param>
        ///<param name="filePath"> File to be encrypted </param>
        public static byte[] encryptionMain(String password, byte[] readBytes)
        {
            //byte[] readBytes = File.ReadAllBytes(filePath); //this throws IO if larger than 2GB--should really make a stream
            byte[] initial = new byte[(int)(BLOCK_LENGTH * (Math.Ceiling((double)readBytes.Length / (double)BLOCK_LENGTH)))]; //rounds to end of block.
            Array.Copy(readBytes, initial, readBytes.Length);
            int initialByteLength = readBytes.Length; //used for CTS.
            readBytes = null;
            GC.Collect();
            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, SALT_LENGTH, NUM_ITERATIONS); //creates random salt for a key
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); //this is cryptographically secure IV


            byte[] initVect = new byte[BLOCK_LENGTH];
            rng.GetBytes(initVect);
            byte[] salt = keyDeriver.Salt;
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH); //gets a key from the password
            byte[] keyHash = getHash(key, salt);//64 bytes--uses same salt as key deriver...this shouldn't be an issue.
            //these are going to have to go on the end...
            BitMatrix ivMat = new BitMatrix(GF_TABLE, SUB_TABLE, initVect, 0); //IV as BM
            BitMatrix[] keys = getKeySchedule(key); //schedule as an array of BMs
            Stopwatch s = new Stopwatch();
            s.Start();
            encrypt(keys, initial, ivMat);
            //initial = encryptWithClasses(key, initial, initVect);
            //Console.WriteLine("Time: " + s.ElapsedMilliseconds);
            byte[] output = packageEncryptionOutput(keyHash, initVect, salt, initial, initialByteLength);
            initial = null;
            GC.Collect();

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="filePath"></param>
        public static void decryptionFromFile(String password, String filePath)
        {
            byte[] readBytes = File.ReadAllBytes(filePath);
            byte[] output = decryptionMain(password, readBytes);
            File.WriteAllBytes(filePath, output);
        }

        ///<summary>
        ///Manages the decryption process.
        ///</summary>
        ///<param name="password"> Password to be decrypted with </param>
        ///<param name="filePath"> File to be decrypted </param>
        public static byte[] decryptionMain(String password, byte[] readBytes) //optimize memory--no need to toBMARRAY this whole thing
        {
            byte[] initial = new byte[(int)(BLOCK_LENGTH * (Math.Ceiling((double)readBytes.Length / (double)BLOCK_LENGTH)))]; //rounds to block
            Array.Copy(readBytes, initial, readBytes.Length);
            int initialByteLength = readBytes.Length; //used for CipherText stealing--makes result same length as input, 
            readBytes = null;                         //regardless of whether its a multiple of the block length.
            GC.Collect();

            byte[] initVect = new byte[BLOCK_LENGTH];
            byte[] salt = new byte[SALT_LENGTH];
            byte[] readHash = new byte[HASH_LENGTH];
            if (initialByteLength < HASH_LENGTH + BLOCK_LENGTH + SALT_LENGTH)
            {
                throw new ApplicationException(); 
            }                                 
            //key management
            Array.Copy(initial, 0, readHash, 0, HASH_LENGTH); //make key, salt, and hash from file.
            Array.Copy(initial, HASH_LENGTH, initVect, 0, BLOCK_LENGTH);
            Array.Copy(initial, HASH_LENGTH + BLOCK_LENGTH, salt, 0, SALT_LENGTH);
            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, salt, NUM_ITERATIONS); //creates random salt for a key
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH);
            byte[] bytesToDecrypt = new byte[initial.Length - HASH_LENGTH - BLOCK_LENGTH - SALT_LENGTH];
            Array.Copy(initial, HASH_LENGTH + BLOCK_LENGTH + SALT_LENGTH, bytesToDecrypt, 0, bytesToDecrypt.Length);
            byte[] keyHash = getHash(key, salt); //64 bytes
            if (!keyHash.SequenceEqual(readHash))
            {
                throw new ApplicationException(); //exception thrown if bad password or file... caught elsewhere.
            }
            //go GC on initial here, GC on bytes to encrypt later
            initial = null;
            GC.Collect();
            BitMatrix ivMat = new BitMatrix(GF_TABLE, SUB_TABLE, initVect, 0); //IV as BM
            BitMatrix[] keys = getKeySchedule(key); //schedule as an array of BMs


            int ShortBytes = BLOCK_LENGTH - initialByteLength % BLOCK_LENGTH;//number of bytes needed to be copied for CTS
            if (ShortBytes >= BLOCK_LENGTH)
            {
                ShortBytes -= BLOCK_LENGTH; //this is if its 16 it makes it 0
            }
            Stopwatch s = new Stopwatch();
            s.Start();
            decrypt(keys, bytesToDecrypt, ivMat, ShortBytes); //backwards is slightly faster I think
            //bytesToDecrypt=decryptWithClasses(key, bytesToDecrypt, initVect);
            //Console.WriteLine("Time: " + s.ElapsedMilliseconds);


            if (bytesToDecrypt.Length==16) //because CTS doesn't work with 1 block stuff. This is the protocol for 1 block.
            {                               //chops off all trailing zero bytes.
                for (int ii=15; ii>=0; ii--)
                {
                    if (bytesToDecrypt[ii]!=0) //this was ALL messed up in the turned in version--initial byte length became like 5
                                               //and the output was tryna be -1000 bytes long.
                    {
                        break; //will chop off any trailing 0x00s, regardless of whether they should be there
                               //if somebody has a sub-16 byte file with a bunch of 0x00 bytes, they will have problems.
                    }                           //But few people will go and need to encrypt that.
                    else
                    {
                        initialByteLength--;
                    }
                }
            }

            byte[] output = new byte[initialByteLength - HASH_LENGTH - BLOCK_LENGTH - SALT_LENGTH];
            Array.Copy(bytesToDecrypt, 0, output, 0, output.Length);
            bytesToDecrypt = null;
            GC.Collect();

            return output;
        }


        ///<summary>
        ///Appends the Hash, IV, and salt to the start of the output.
        ///</summary>
        ///<param name="keyHash"> Hash of the encryption key </param>
        ///<param name="initVect"> Initialization Vector </param>
        ///<param name="salt"> Salt to be added</param>
        ///<param name="fin"> Output to be appended onto </param>
        ///<param name="initialByteLength"> Initial length of the input, to be truncated down to. </param>
        ///<returns> The full byte array to be outputted. </returns>
        public static byte[] packageEncryptionOutput(byte[] keyHash, byte[] initVect, byte[] salt,
                                                     byte[] fin, int initialByteLength)
        {   //this is a truncate step for CTS...we need a 1 block check somewhere.
            if (initialByteLength < BLOCK_LENGTH && initialByteLength > 0)
            {
                initialByteLength = BLOCK_LENGTH; //currently keeps the full block for 1 block. plaintext is 0x00.
            }
            byte[] output = new byte[initialByteLength + HASH_LENGTH + BLOCK_LENGTH + SALT_LENGTH];
            Array.Copy(keyHash, 0, output, 0, keyHash.Length);
            Array.Copy(initVect, 0, output, HASH_LENGTH, initVect.Length);
            Array.Copy(salt, 0, output, HASH_LENGTH + BLOCK_LENGTH, salt.Length);
            Array.Copy(fin, 0, output, HASH_LENGTH + BLOCK_LENGTH + SALT_LENGTH, initialByteLength);
            return output;
        }


        /// <summary>
        /// Runs the encryption algorithm in CBC mode.
        /// </summary>
        /// <param name="key"> The full key schedule </param>
        /// <param name="toEncrypt"> The byte array to be encrypted </param>
        /// <param name="iv"> The initialization vector </param>
        public static void encrypt(BitMatrix[] key, byte[] toEncrypt, BitMatrix iv)
        {
            if (toEncrypt.Length == 0)
            {
                return;
            }
            BitMatrix matrix = new BitMatrix(GF_TABLE, SUB_TABLE, toEncrypt, 0); //first matrix is seperate 
            matrix.addRoundKey(iv);                                              //because it operates off IV
            encryptSingle(key, matrix); //for CBC
            for (int jj = 1; jj < toEncrypt.Length / BLOCK_LENGTH; jj++)
            {
                BitMatrix oldMatrix = matrix;
                matrix = new BitMatrix(GF_TABLE, SUB_TABLE, toEncrypt, BLOCK_LENGTH * jj);
                matrix.addRoundKey(oldMatrix); //CBC
                encryptSingle(key, matrix);
            }
            if (toEncrypt.Length > BLOCK_LENGTH) //for CipherText stealing
            {   //CTS encrypting in Cipher Block Chaining there is an implicit copy in the XOR
                //you just flip the last two blocks and truncate.
                swapElements(toEncrypt, toEncrypt.Length - (2 * BLOCK_LENGTH), toEncrypt.Length - BLOCK_LENGTH, BLOCK_LENGTH);
            }
        }

        public static byte[] encryptWithClasses(byte[] key, byte[] toEncrypt, byte[] iv)
        {
            byte[] encrypted = new byte[(int) (Math.Ceiling((double)toEncrypt.Length / 16.0)) * 16];
            AesManaged aes = new AesManaged();
            ICryptoTransform ict=aes.CreateEncryptor(key, iv);
            ict.TransformBlock(toEncrypt, 0, toEncrypt.Length, encrypted, 0);
            return encrypted;
        }

        public static byte[] decryptWithClasses(byte[] key, byte[] toDecrypt, byte[] iv)
        {
            byte[] decrypted = new byte[(int)(Math.Ceiling((double)toDecrypt.Length / 16.0)) * 16];
            AesManaged aes = new AesManaged();
            ICryptoTransform ict = aes.CreateDecryptor(key, iv);
            ict.TransformBlock(toDecrypt, 0, toDecrypt.Length, decrypted, 0);
            return decrypted;
        }

        /// <summary>
        /// Encrypts a single block.
        /// </summary>
        /// <param name="key"> The full key schedule </param>
        /// <param name="matrix"> The block to be encrypted </param>
        public static void encryptSingle(BitMatrix[] key, BitMatrix matrix)
        {
            matrix.addRoundKey(key[0]);
            for (int ii = 1; ii < ROUNDS - 1; ii++)
            {
                matrix.subBytes();
                matrix.shiftAndMix();
                matrix.addRoundKey(key[ii]);
            }
            matrix.subBytes();
            matrix.shiftRows();
            matrix.addRoundKey(key[ROUNDS - 1]);
        }

        /// <summary>
        /// Runs the decryption process in CBC mode.
        /// </summary>
        /// <param name="key"> The full key schedule </param>
        /// <param name="toDecrypt"> The byte array to be decrypted </param>
        /// <param name="iv"> The initialization vector </param>
        /// <param name="replaceBytes"> The number of bytes to be copied over for CTS </param>
        public static void decrypt(BitMatrix[] key, byte[] toDecrypt, BitMatrix iv, int replaceBytes)
        {
            if (toDecrypt.Length < 17)
            {
                if (toDecrypt.Length > 0) //no CTS for 1 block
                {
                    BitMatrix m = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, 0);
                    decryptSingle(key, m);
                    m.addRoundKey(iv);
                }
                return;
            }
            //all for CTS....flip the last two blocks, padded with 0s
            swapElements(toDecrypt, toDecrypt.Length - 2 * BLOCK_LENGTH, toDecrypt.Length - BLOCK_LENGTH, BLOCK_LENGTH);
            BitMatrix secondToLastMatrix = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, toDecrypt.Length - 2 * BLOCK_LENGTH);
            BitMatrix lastMatrix = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, toDecrypt.Length - BLOCK_LENGTH);
            decryptSingle(key, lastMatrix); //decrypt the last block.
            byte[] lastBytes = lastMatrix.getBytes();
            byte[] secondBytes = secondToLastMatrix.getBytes();
            for (int ii = 15; ii >= BLOCK_LENGTH - replaceBytes; ii--)  //move bytes one by one, to create two full blocks
            {
                secondBytes[ii + secondToLastMatrix.startValue] = lastBytes[ii + lastMatrix.startValue];
            }
            lastMatrix.addRoundKey(secondToLastMatrix); //XOR last block for result. The excess 0x00 bytes will be truncated.
            decryptSingle(key, secondToLastMatrix); //decrypt 2nd to last

            BitMatrix matrix = null; //prevents having to create two bit Matrices
            if (toDecrypt.Length > 2 * BLOCK_LENGTH) //if its originally 33, the it should pad correct to 48. 32 is 32.
            {
                matrix = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, toDecrypt.Length - 48);
                secondToLastMatrix.addRoundKey(matrix); //XOR 2nd to last
            }
            else //if its under 33
            {
                secondToLastMatrix.addRoundKey(iv);
                return;
            }
            for (int jj = toDecrypt.Length / BLOCK_LENGTH - 3; jj > 0; jj--) //decrypts end to front.
            {
                BitMatrix nextMatrix = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, BLOCK_LENGTH * jj - BLOCK_LENGTH);
                decryptSingle(key, matrix);
                matrix.addRoundKey(nextMatrix); //for CBC
                matrix = nextMatrix;
            }
            matrix = new BitMatrix(GF_TABLE, SUB_TABLE, toDecrypt, 0);
            decryptSingle(key, matrix);
            matrix.addRoundKey(iv); //for CBC
        }

        /// <summary>
        /// Decrypts a single block.
        /// </summary>
        /// <param name="key"> The full key schedule </param>
        /// <param name="matrix"> The block to be decrypted </param>
        public static void decryptSingle(BitMatrix[] key, BitMatrix matrix) //decrypts a single using the block. No CBC here
        {
            if (matrix == null)
            {
                throw new NullReferenceException("Decrypting null matrix");
            }
            matrix.addRoundKey(key[ROUNDS - 1]);
            matrix.invShiftRows();
            for (int ii = ROUNDS - 2; ii > 0; ii--)
            {
                matrix.invSubBytes();
                matrix.addRoundKey(key[ii]);
                matrix.invShiftAndMix();
            }
            matrix.invSubBytes();
            matrix.addRoundKey(key[0]);
        }

        /// <summary>
        /// Creates the full key schedule.
        /// </summary>
        /// <param name="key"> The single block key to be expanded. </param>
        /// <returns> The key schedule </returns>
        public static BitMatrix[] getKeySchedule(byte[] key) //this works
        {
            if (key.Length!=BLOCK_LENGTH)
            {
                throw new ArgumentException("Error in key schudule generation");
            }
            byte[][] schedule = new byte[BLOCK_LENGTH/WORD_LENGTH * ROUNDS][];
            for (int ii = 0; ii < schedule.Length; ii++)
            {
                schedule[ii] = new byte[WORD_LENGTH]; //initializes the schedule as an array of 4 byte arrays. This is because
                                                      //it creates the schedule word by word.
            }

            byte[] roundConstants = generateRoundConstants(); //creates the round constants. These are single bytes that are XORed
            for (int ii = 0; ii < BLOCK_LENGTH; ii++)         //with the first byte of the first word of the first 
            {
                schedule[ii / WORD_LENGTH][ii % WORD_LENGTH] = key[ii]; //gets the words from the key and throws them in the first four slots.
            }
            for (int ii = WORD_LENGTH; ii < schedule.Length; ii++) //these are words
            {
                if (ii % WORD_LENGTH != 0) //if it is the 2nd, 3rd, or 4th word of the key, it is just the 1st word XORed
                {                          //with the equivalent word of the previous block.
                    for (int jj = 0; jj < schedule[ii].Length; jj++)
                    {
                        schedule[ii][jj] = (byte)(schedule[ii - WORD_LENGTH][jj] ^ schedule[ii - 1][jj]); //xor four before with the first word of this key
                    }
                }
                else //the 1st word of each key is more difficult.
                {
                    schedule[ii] = (byte[])schedule[ii - 1].Clone(); //otherwise there will be a reference pass
                    byteRotate(schedule[ii]); //rotate by 1 BYTE not bit
                    for (int jj = 0; jj < schedule[ii].Length; jj++)
                    {
                        schedule[ii][jj] = SUB_TABLE.getByte(schedule[ii][jj]); //subByte the whole box
                    }
                    schedule[ii][0] = (byte)(schedule[ii][0] ^ roundConstants[ii / WORD_LENGTH - 1]); //xor the constants to the top of the words
                    for (int jj = 0; jj < schedule[ii].Length; jj++) //theres a -1 on he round constants because there is no RC for the original 4 word key
                    {
                        schedule[ii][jj] = (byte)(schedule[ii][jj] ^ schedule[ii - WORD_LENGTH][jj]); //mod it with the previous first word.
                    }
                }
            }
            BitMatrix[] sched = new BitMatrix[ROUNDS]; //converts the created key to bit matrices.
            for (int ii = 0; ii < sched.Length; ii++) //convert the data into bit matrices
            {
                sched[ii] = new BitMatrix(GF_TABLE, SUB_TABLE, schedule[WORD_LENGTH * ii], schedule[WORD_LENGTH * ii + 1], schedule[WORD_LENGTH * ii + 2], schedule[WORD_LENGTH * ii + 3]);
            }
            return sched;
        }

        /// <summary>
        /// Generates the round constants using GF(2^8) arithmetic.
        /// </summary>
        /// <returns> The round constants. </returns>
        public static byte[] generateRoundConstants() //creates the round constants used in the key generation
        {
            byte[] constants = new byte[ROUNDS - 1]; //the first round uses the 128 bit key so no constant for that.
            constants[0] = 1;
            for (int ii = 1; ii < constants.Length; ii++)
            {
                constants[ii] = GF_TABLE.multiply(constants[ii - 1], 0x02);
            }
            return constants;
        }

        /// <summary>
        /// Rotates a byte array by one full byte.
        /// </summary>
        /// <param name="b"> The byte array to be rotated </param>
        public static void byteRotate(byte[] b) //circularly rotates the array--if shift rows goes back to a [][]
                                                //structure, then there is overlap here.
        {
            byte temp = b[0];
            for (int ii = 0; ii < b.Length - 1; ii++)
            {
                b[ii] = b[ii + 1];
            }
            b[b.Length - 1] = temp;
        }

        /// <summary>
        /// Creates the Hash of the key.
        /// </summary>
        /// <param name="key"> The key to be hashed. </param>
        /// <param name="salt"> The salt hashed with the key </param>
        /// <returns> The hash of the key. </returns>
        public static byte[] getHash(byte[] key, byte[] salt)
        {
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(key, salt, NUM_ITERATIONS); //The hash is PBKDF2 on the key
            return rdb.GetBytes(HASH_LENGTH);                                           //with the same salt as before.
        }

        /// <summary>
        /// Swaps a pair of blocks in a byte array
        /// </summary>
        /// <param name="array"> The array in which the swap is taking place</param>
        /// <param name="block1S"> The start of the first block.</param>
        /// <param name="block2S"> The start of the second block. </param>
        /// <param name="length"> The length of the blocks being swapped. </param>
        public static void swapElements(byte[] array, int block1S, int block2S, int length) //need a bunch of exceptions here--but exceptions are slow
        {
            for (int ii = 0; ii < length; ii++)
            {
                byte temp = array[block1S + ii];
                array[block1S + ii] = array[block2S + ii];
                array[block2S + ii] = temp;
            }
        }
    }
}
