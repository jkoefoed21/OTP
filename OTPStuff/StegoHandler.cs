using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using encryption;
using System.IO;
using System.Threading;
using System.Diagnostics;


namespace Stego_Stuff
{
    static class StegoHandler
    {
        //IMAGES ARE DEFINED AS COL, ROW
        //WHEN LOADED INTO AN ARRAY, IMAGES ARE LITTLE ENDIAN. byte 54 is B, 55 is G, 56 is R, 57 is A, then 58 is B2.
        //in addition, 54-57 is the lower, left corner. this contrasts with Image.getpixel(), which starts in the top left.
        //knowing this, we can speed the whole process up
        //would have to decide whether we would want to reindex and rebuild the full system or still focus on a top left oriented approach.
        
        /// <summary>
        /// The length of 128 bits in bytes
        /// </summary>
        public static readonly int BLOCK_LENGTH = 16; //bytes not bits

        /// <summary>
        /// The length of the password hash.
        /// </summary>
        public static readonly int HASH_LENGTH = 64; //bytes

        /// <summary>
        /// The Length of the salt.
        /// </summary>
        public static readonly int SALT_LENGTH = 64; //bytes not bits

        /// <summary>
        /// The number of bits in a byte=8
        /// </summary>
        public static readonly int BITS_IN_BYTE = 8;

        /// <summary>
        /// How many bytes it will take to contain one bit--USE POWER OF TWO BETWEEN 1 AND 256--otherwise, risks cryptanalysis because greater chance in 0 or 1 position
        /// </summary>
        public static readonly int STEGO_DENSITY = 8;
        
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

        /// <summary>
        /// The number of bytes in a single pixel--in ARGB is 3.
        /// </summary>
        public static readonly byte BYTES_IN_PX = 3;

        /// <summary>
        /// This saves so much time and space in the code.
        /// </summary>
        public static readonly int START_LENGTH = BLOCK_LENGTH + HASH_LENGTH + SALT_LENGTH;

        /// <summary>
        /// The Number of iterations used for the PBKDF2. This slows the program down a lot
        /// but it is good that it does, because it makes the hash, iv cryptographically secure.
        /// </summary>
        public static readonly int NUM_ITERATIONS = 30000; //slows the algorithm down by about a second...for security though
        
        
        //This is a script to ensure that the generate noise function in fact generates noise. Need to check Alpha somehow.
        
        public static void Main(String[] args)
        {
            Bitmap i = new Bitmap("C:\\Users\\JK\\Pictures\\b2.png");
            byte[] b = imageToBytes(i);
            Console.WriteLine("Height: " + i.Height);
            Console.WriteLine("Width: " + i.Width);
            Console.WriteLine(b.Length);
            Console.WriteLine(4 * i.Width * i.Height + 54);
            for (int ii = 54; ii < 150; ii+=4)
            {
                Console.Write((ii - 54) / 4);
                Console.Write("__");
                Console.Write("{0:X}", b[ii]);
                Console.Write("__");
                Console.Write("{0:X}", b[ii+1]);
                Console.Write("__");
                Console.Write("{0:X}", b[ii+2]);
                Console.Write("__");
                Console.Write("{0:X}", b[ii+3]);

                Console.WriteLine();
            }
            Console.WriteLine("{0:X}",i.GetPixel(0, i.Height-1).ToArgb());
            Console.ReadKey();
            /*Bitmap i2 = new Bitmap("C:\\Users\\JK\\Pictures\\monocolor.png");
            int r = 0;
            int g = 0;
            int b = 0;
            for (int ii = 0; ii < i.Width; ii++)
            {
                //Console.Write("{0:X}", i2.GetPixel(ii, i.Height-1).ToArgb());
                //Console.Write("__");
                Console.Write("{0:X}", i.GetPixel(ii, i.Height - 1).ToArgb());
                Console.Write("___");
                for (int jj = 0; jj < i.Height; jj++)
                {
                    if (i.GetPixel(ii, jj).R != i2.GetPixel(ii, jj).R)
                    {
                        //Console.Write("R");
                        r++;
                    }
                    if (i.GetPixel(ii, jj).G != i2.GetPixel(ii, jj).G)
                    {
                        //Console.Write("G");
                        g++;
                    }
                    if (i.GetPixel(ii, jj).B != i2.GetPixel(ii, jj).B)
                    {
                        //Console.Write("B");
                        b++;
                    }
                }
                //Console.WriteLine();
            }
            int tally = 0;
            for (int ii = 0; ii < i.Height * i.Width * 3; ii++)
            {
                if (readPixel(ii, i) == 1)
                {
                    tally++;
                }
            }
            Console.WriteLine("1s: " + tally + " 0s: " + (i.Height * i.Width*3 - tally));
            Console.WriteLine("R change: " + r + " G change: " + g + " B change: " + b);
            Console.ReadKey();*/
        }
        

        /// <summary>
        /// The main implantation method
        /// </summary>
        /// <param name="password"> The password being used for implantation</param>
        /// <param name="b">The image being implanted within</param>
        /// <param name="msg">The message to be encrypted</param>
        public static Bitmap implantMain(String password, Bitmap b, byte[] msg)//, String finalPath)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            //Bitmap b = new Bitmap(imgPath); //throws FileNotFoundException
            //byte[] readBytes = File.ReadAllBytes(msgPath); //this throws IO if larger than 2GB--should really make a stream
            byte[] messBytes = msg;//addEOF(msg);

            if (messBytes.Length>(availableBytes(b.Height*b.Width)+EOF1_LENGTH+2*BLOCK_LENGTH+1+AES.START_LENGTH)) //this needs to be tested rigorously eventually
            {
               throw new ArgumentException("Message is too long");
            }

            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, SALT_LENGTH, NUM_ITERATIONS); //creates random salt for a key
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); //this is cryptographically secure IV
            byte[] initVect = new byte[BLOCK_LENGTH];
            rng.GetBytes(initVect);
            byte[] salt = keyDeriver.Salt;
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH); //gets a key from the password
            byte[] keyHash = getHash(key, salt);//64 bytes--uses same salt as key deriver...this shouldn't be an issue.
            BitMatrix[] keySched = AES.getKeySchedule(key);
            Console.WriteLine("Time before noise: " + s.ElapsedMilliseconds);
            s.Restart();
            b=generateNoise(b);
            Console.WriteLine("Noise: " + s.ElapsedMilliseconds);
            s.Restart();
            implantBlock(b, 0, keyHash);
            implantBlock(b, keyHash.Length, initVect);
            implantBlock(b, keyHash.Length+initVect.Length, salt);
            Console.WriteLine("Block implant time: " + s.ElapsedMilliseconds);
            s.Restart();
            implantMessage(b, keySched, messBytes, initVect, START_LENGTH*BITS_IN_BYTE);
            Console.WriteLine("Message time: " + s.ElapsedMilliseconds);
            return b;
            //b.Save(finalPath, ImageFormat.Png);
        }

        /// <summary>
        /// The main extraction method
        /// </summary>
        /// <param name="password">The password being extracted with</param>
        /// <param name="b">The image being extracted from</param>
        /// <returns>The extracted message</returns>
        public static byte[] extractMain(String password, Bitmap b)//int[] image)
        {
            if(b.Height*b.Width<START_LENGTH*2) 
            {
                throw new ArgumentException("File is too small to read");
            }
            byte[] initVect = new byte[BLOCK_LENGTH];
            byte[] salt = new byte[SALT_LENGTH];
            byte[] readHash = new byte[HASH_LENGTH];
            
            extractBlock(b, 0, readHash);
            extractBlock(b, readHash.Length, initVect);
            extractBlock(b, readHash.Length + initVect.Length, salt);

            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, salt, NUM_ITERATIONS);
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH);
            BitMatrix[] keySched = AES.getKeySchedule(key);
            byte[] compHash = getHash(key, salt); //could potentially speed this up by eliminating this
                                                  //as long as client knows to check before calling.
            if(!readHash.SequenceEqual(compHash))
            {
                throw new ArgumentException("Wrong Password or not a Stego File");
            }
            Stopwatch s = new Stopwatch();
            s.Start();
            byte[] messBytes=extractMessage(b,  keySched, initVect, START_LENGTH*BITS_IN_BYTE);
            Console.WriteLine("Msg. Extract Time: " + s.ElapsedMilliseconds);
            //Console.WriteLine(messBytes.Length);
            //byte[] finalMessBytes = new byte[messBytes.Length - EOF1_LENGTH]; //when pulling message, includes EOF1 but not EOF2
            //Array.Copy(messBytes, finalMessBytes, finalMessBytes.Length);
            return messBytes;
        }

        /// <summary>
        /// Checks the password to see if it is the right password
        /// </summary>
        /// <param name="password"> The password being checked</param>
        /// <param name="b">The image being checked</param>
        /// <returns>True if the image contains a hidden message</returns>
        public static bool checkHash(String password, Bitmap b)
        {
            byte[] readHash = new byte[HASH_LENGTH];
            byte[] salt = new byte[SALT_LENGTH];
            extractBlock(b, 0, readHash);
            extractBlock(b, readHash.Length + BLOCK_LENGTH, salt);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, NUM_ITERATIONS);
            byte[] key = rfc.GetBytes(BLOCK_LENGTH);
            if(readHash.SequenceEqual(getHash(key, salt)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Implants a block sequentially in an image
        /// </summary>
        /// <param name="b">The image being implanted within</param>
        /// <param name="start">The byte index of the start of the implantation within the image</param>
        /// <param name="array">The array being implanted</param>
        public static void implantBlock(Bitmap b, int start, byte[] array)
        {
            for (int ii = 0; ii < array.Length * 8; ii++)
            {
                modifyPixel(start*8+ii, b, (byte)(array[ii / 8] >> 7 - (ii % 8)) % 2);
            }
        }

        /// <summary>
        /// Extracts a block sequentially from an image
        /// </summary>
        /// <param name="b">The image being extracted from</param>
        /// <param name="start">The byte index of the start of the extraction within the image. Defined as Px# *3</param>
        /// <param name="array">The array to extract to</param>
        public static void extractBlock(Bitmap b, int start, byte[] array)
        {
            for (int ii = start; ii < start+array.Length; ii++)
            {
                byte nextNum = 0;
                for (int jj = 0; jj < 8; jj++)
                {
                    nextNum = (byte)(nextNum << 1);
                    nextNum += readPixel(ii * 8 + jj, b);
                }
                array[ii-start] = nextNum;
            }
        }

        /// <summary>
        /// Overwrites all the LSBs of an image with random bits.
        /// </summary>
        /// <param name="b">The image </param>
        /*public static void generateNoise(Bitmap b, bool overload) //pretty time expensive
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[b.Height * b.Width*BYTES_IN_PX];
            rng.GetBytes(randomBytes);
            for (int ii=0; ii<randomBytes.Length;  ii++)
            {
                randomBytes[ii] = (byte)(randomBytes[ii] % 2);
            }
            int[] rbs = new int[b.Height * b.Width];
            for (int ii=0; ii<rbs.Length; ii++)
            {
                rbs[ii] = (randomBytes[3 * ii] << 16) + (randomBytes[3 * ii + 1] << 8) + (randomBytes[3 * ii + 2]);
            }
            for (int ii=0; ii<b.Height; ii++)
            {
                for (int jj=0; jj<b.Width; jj++)
                {
                    int color=b.GetPixel(jj,ii).ToArgb();
                    b.SetPixel(jj, ii, Color.FromArgb(color ^ rbs[ii * b.Width + jj]));
                }
            }
            Console.WriteLine("New time=" +s.ElapsedMilliseconds);
            s.Restart();
        }*/

        public static Bitmap generateNoise(Bitmap b) //very cheap
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            byte[] bytes=imageToBytes(b);
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[b.Height * (int)(Math.Ceiling(b.Width/4.0)*4) * BYTES_IN_PX];
            rng.GetBytes(randomBytes);
            for (int ii = 0; ii < randomBytes.Length; ii++)
            {
                randomBytes[ii] = (byte)(randomBytes[ii] % 2);
            }
            if (bytes.Length > b.Height * b.Width * 4)//if records ALPHA
            {
                int rbIndex = 0;
                for (int ii = 54; ii < bytes.Length; ii++)
                {
                    if((ii%4!=1 && BitConverter.IsLittleEndian) || (ii%4!=2 && !BitConverter.IsLittleEndian)) //this is a little endian system never forget
                    {
                        bytes[ii] ^= randomBytes[rbIndex];
                        rbIndex++;
                    }
                }
            }
            else
            {
                for (int ii = 54; ii < bytes.Length; ii++)
                {
                    bytes[ii] ^= randomBytes[ii - 54];
                }
            }
            
            Bitmap newB = (Bitmap)bytesToImage(bytes);
            b.Dispose();
            Console.WriteLine("New time=" + s.ElapsedMilliseconds);
            s.Restart();
            return newB;
        }


        /// <summary>
        /// Implants a message in an image--does not handle encryption of inner message.
        /// </summary>
        /// <param name="b"> The image being implanted within </param>
        /// <param name="keySched"> The key schedule being used </param>
        /// <param name="message"> The message being implanted </param>
        /// <param name="initVect"> The IV of the stego implantation </param>
        /// <param name="startPosition"> The byte position, from 0 to #Px*3, where the message starts</param>
        public static void implantMessage(Bitmap b, BitMatrix[] keySched, byte[] message, byte[] initVect, int startPosition) //add start place param
        {
            BitMatrix iv = new BitMatrix(AES.GF_TABLE, AES.SUB_TABLE, initVect, 0);
            for (int ii = 0; ii < Math.Floor((double) message.Length / 2.0); ii++)
            {
                //encryption of message takes 5% of time. Probably more on extract.
                AES.encryptSingle(keySched, iv); //operates as a stream cipher--XTS mode I think? Who knows.
                for (int jj = 0; jj < BLOCK_LENGTH; jj++) //because implants in 2 byte chunks, b/c 16 bits--16 bytes=128 bits AES
                {
                    modifyPixel(startPosition + STEGO_DENSITY *  (2 * BITS_IN_BYTE * ii + jj) + initVect[jj]%STEGO_DENSITY, b, getBitFromByte(message[ii*2+jj/8], jj%8));
                }
            }
            if (message.Length%2==1) //will encode last byte which is a half block
            {
                AES.encryptSingle(keySched, iv); //operates as a stream cipher--XTS mode I think? Who knows.
                int ii=(int) Math.Floor((double)message.Length / 2.0);
                for (int jj = 0; jj < BLOCK_LENGTH/2; jj++)
                {  
                    modifyPixel(startPosition * BITS_IN_BYTE + STEGO_DENSITY * (2 * BITS_IN_BYTE * ii + jj) + initVect[jj]%STEGO_DENSITY, b, getBitFromByte(message[ii * 2 + jj / 8], jj % 8));
                }
            }
        }

        /// <summary>
        /// Extracts a Message from an image, reading until the end.
        /// </summary>
        /// <param name="b"> The image being extracted from</param>
        /// <param name="keySched"> The key schedule being used </param>
        /// <param name="initVect"> The IV of the stego</param>
        /// <param name="startPosition"> The byte position, from 0 to #Px*3, where extraction starts</param>
        /// <returns> An array containing the message from the start to the end of the image. Still have to chop at EOF. </returns>
        public static byte[] extractMessage(Bitmap b, BitMatrix[] keySched, byte[] initVect, int startPosition)
        {
            byte[] message = new byte[(BYTES_IN_PX * (b.Height * b.Width) - startPosition) / STEGO_DENSITY];
            BitMatrix iv = new BitMatrix(AES.GF_TABLE, AES.SUB_TABLE, initVect, 0);
            for (int ii = 0; ii < (b.Height * b.Width * BYTES_IN_PX - startPosition) / (STEGO_DENSITY * BITS_IN_BYTE * 2); ii++)
            {
                //printByteArray(message);
                AES.encryptSingle(keySched, iv); //should probably implement this with classes eventually.
                for (int jj = 0; jj < BLOCK_LENGTH / 2; jj++)
                {
                    if (readPixel(startPosition + STEGO_DENSITY * (2 * BITS_IN_BYTE * ii + jj) + initVect[jj] % STEGO_DENSITY, b) == 1)//256 is there to provide room for stream cipher
                    {
                        message[2*ii] = stickBitInByte(message[2*ii], jj);
                    }
                    //Console.WriteLine(message[2 * ii]);
                }
                for (int jj = BLOCK_LENGTH / 2; jj < BLOCK_LENGTH; jj++)
                {
                    if (readPixel(startPosition + STEGO_DENSITY * (2 * BITS_IN_BYTE * ii + jj) + initVect[jj] % STEGO_DENSITY, b) == 1)
                    {
                        message[2*ii+1] = stickBitInByte(message[2*ii+1], jj - BITS_IN_BYTE);
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// Inserts a single bit into a pixel
        /// </summary>
        /// <param name="valueNum"> The position to insert the bit--indexed from 0 to 3 times total pixels </param>
        /// <param name="b"> The image being inserted </param>
        /// <param name="toEncode"> The bit to be encoded </param>
        public static void modifyPixel(int valueNum, Bitmap b, int toEncode) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / BYTES_IN_PX;
            int pixVal = b.GetPixel(pixelNum % b.Width, pixelNum / b.Width).ToArgb();
            
            if (toEncode==1)
            {
                toEncode = toEncode << (BITS_IN_BYTE * ((BYTES_IN_PX - 1) - (valueNum % BYTES_IN_PX)));
                pixVal |= toEncode;
            }
            else
            {
                int cleaning = 1 << BITS_IN_BYTE * (((BYTES_IN_PX - 1) - (valueNum % BYTES_IN_PX)));
                pixVal = (pixVal & (-1 - cleaning));
            }
            /*
            toEncode = toEncode << (BITS_IN_BYTE * ((BYTES_IN_PX - 1) - (valueNum % BYTES_IN_PX))); 
            int cleaning = 1 << BITS_IN_BYTE * (((BYTES_IN_PX - 1) - (valueNum % BYTES_IN_PX))); //only works because cleaning will never be in the top bit, so no overflow below
            pixVal = (pixVal & (-1 - cleaning)) | toEncode; //So apparently -1 is 0xFFFFFFFF in c# signed ints SUCK
            */
            b.SetPixel(pixelNum % b.Width, pixelNum / b.Width, Color.FromArgb(pixVal));
        }

        /// <summary>
        /// Reads a single bit from a pixel
        /// </summary>
        /// <param name="valueNum"> The position to read the bit--indexed from 0 to 3 times total pixels </param>
        /// <param name="b"> The image being inserted </param>
        /// <returns> Returns a byte of either 0 or 1 </returns>
        public static byte readPixel(int valueNum, Bitmap b) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / BYTES_IN_PX;
            Color pixel = b.GetPixel(pixelNum % b.Width, pixelNum / b.Width);
            byte read = 0;
            if (valueNum%BYTES_IN_PX==0)
            {
                read = pixel.R;
            }
            else if(valueNum%BYTES_IN_PX==1)
            {
                read = pixel.G;
            }
            else
            {
                read = pixel.B;
            }
            return (byte) (read % 2);
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
        /// Prints an array of bytes line-separated
        /// </summary>
        /// <param name="byteArray"> The byte array to be printed</param>
        public static void printByteArray(byte[] byteArray)
        {
            for (int ii=0; ii<byteArray.Length; ii++)
            {
                Console.Write("{0:X}", byteArray[ii]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints an int as a series of 0s and 1s
        /// </summary>
        /// <param name="toPrint"> The int to be printed </param>
        public static void printIntAsBits(int toPrint)
        {
            for (int ii=0; ii<32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
            }
        }

        /// <summary>
        /// Prints a long as a series of 0s and 1s
        /// </summary>
        /// <param name="toPrint"> The long to be printed </param>
        public static void printIntAsBits(long toPrint)
        {
            for (int ii = 0; ii < 32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
                Console.Write(" | ");
            }
        }

        /// <summary>
        /// Prints a ulong as a series of 0s and 1s
        /// </summary>
        /// <param name="toPrint"> The ulong to be printed </param>
        public static void printIntAsBits(ulong toPrint)
        {
            for (int ii = 0; ii < 32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
                Console.Write(" | ");
            }
        }

        /// <summary>
        /// Gets a bit from a byte, indexed 0 as MSB, 7 as LSB
        /// </summary>
        /// <param name="b"> The byte being extracted from </param>
        /// <param name="index"> The index being extracted from</param>
        /// <returns> the bit, 0 or 1, as an int </returns>
        public static byte getBitFromByte(byte b, int index) //this is indexed where 0 is MSB, 7 is LSB
        {
            return (byte) ((b >> ((BITS_IN_BYTE-1) - index)) % 2);
        }

        /// <summary>
        /// Sticks a 1 into a byte at a specified index
        /// </summary>
        /// <param name="b"> The byte being modified </param>
        /// <param name="index"> The index to stick a one at </param>
        /// <returns> The altered byte </returns>
        public static byte stickBitInByte(byte b, int index) //can't add a 0 to a byte--would just be do nothing
        {
            byte add = 1;
            b += (byte) (add << ((BITS_IN_BYTE-1) - (byte) index));
            return b;
        }

        /// <summary>
        /// Archaic--when wasn't operating in-place
        /// </summary>
        /// <param name="b"> The image</param>
        /// <returns> and int array of the image </returns>
        public static int[] imageToIntArray(Bitmap b)
        {
            int[] output = new int[b.Height*b.Width];
            for (int ii=0; ii<b.Height*b.Width; ii++)
            {
                output[ii] = b.GetPixel(ii % b.Width, ii / b.Width).ToArgb();
            }
            return output;
        }
        
        /// <summary>
        /// Also archaic--not in place
        /// </summary>
        /// <param name="intArr"> An int array representing an image </param>
        /// <param name="b"> An image pointer to be reset </param>
        public static void setImageFromIntArray(int[] intArr, Bitmap b)
        {
            for (int ii = 0; ii < b.Height * b.Width; ii++)
            {
                b.SetPixel(ii % b.Width, ii / b.Width, Color.FromArgb(intArr[ii]));
            }
        }

        /// <summary>
        /// Converts a string input to a byte output with EOF characters for the stego to understand.
        /// </summary>
        /// <param name="message"> The string to be encoded </param>
        /// <returns> The bytes to be implanted </returns>
        public static byte[] stringToByteArrayWithEOF(string message)
        {
            char[] messChars=message.ToCharArray(); //base on mod 2 
            byte[] messBytes = new byte[messChars.Length+EOF1_LENGTH+1]; //could refactor this using addEOF(byte[]) but would be slower
            for (int ii=0; ii<messChars.Length; ii++)
            {
                messBytes[ii] = (byte)messChars[ii];
            }
            //lay down 4s, then an EOF
            for (int ii=0; ii<EOF1_LENGTH; ii++)//MAGIC
            {
                messBytes[messChars.Length + ii] = EOF_CHAR1;
            }
            messBytes[messChars.Length + EOF1_LENGTH] = EOF_CHARFINAL;
            return messBytes;
        }
        
        /// <summary>
        /// Adds a EOF to existing byte array. Must allocate and copy array
        /// </summary>
        /// <param name="message"> The bytes without EOF</param>
        /// <returns> The bytes w/ EOF</returns>
        public static byte[] addEOF(byte[] message)
        {
            //Console.WriteLine(message.Length);
            byte[] bytesWEOF = new byte[message.Length + EOF1_LENGTH + 1 + 2*BLOCK_LENGTH];
            Array.Copy(message, bytesWEOF, message.Length);
            for (int ii = 0; ii < EOF1_LENGTH; ii++)//MAGIC
            {
                bytesWEOF[message.Length + ii] = EOF_CHAR1;
            }
            bytesWEOF[message.Length + EOF1_LENGTH] = EOF_CHARFINAL;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] r = new byte[BLOCK_LENGTH*2]; //adds 32 bytes of random data, but ensures that the EOF is not damaged by random data after the image.
            rng.GetBytes(r);
            Array.Copy(r, 0, bytesWEOF, message.Length + EOF1_LENGTH + 1, r.Length);
            return bytesWEOF;
        }

       public static byte[] chopEOF(byte[] message)
       {
            int endCount = 0;
            for (int ii=0; ii<message.Length; ii++)
            {
                if (endCount>=EOF1_LENGTH&&message[ii]==EOF_CHARFINAL)
                {
                    byte[] final = new byte[ii - EOF1_LENGTH];
                    Array.Copy(message, final, final.Length);
                    return final;
                }

                if(message[ii]==EOF_CHAR1)
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

        /// <summary>
        /// Converts an int to a uint
        /// </summary>
        /// <param name="toConvert"> An int to convert to a uint </param>
        /// <returns> A uint of the int</returns>
        public static uint intToUInt(int toConvert)
        {
            unchecked
            {
                return (uint)toConvert;
            }
        }

        public static byte[] imageToBytes(Image i) 
        {
            Image i2 = new Bitmap(i.Width, i.Height, PixelFormat.Format32bppArgb);
            using (var gr = Graphics.FromImage(i2))
            {
                gr.DrawImage(i, new Rectangle(0, 0, i2.Width, i2.Height));
            }
            //Console.WriteLine("NUM PX "+i.Height * i.Width);
            using (MemoryStream m = new MemoryStream())
            {
                i2.Save(m, ImageFormat.Bmp);
                /*Console.WriteLine("NUM BYTES PREDICTED: " + (4 * Math.Ceiling((double)i.Width / 4.0) * 4 * i.Height + 54));
                Console.WriteLine("NUM BYTES "+m.ToArray().Length);*/
                return m.ToArray();
            }
        }

        public static Image bytesToImage(byte[] bytes)
        {
            MemoryStream m = new MemoryStream(bytes);
            return Image.FromStream(m);
        }

        /// <summary>
        /// Calculates the available bytes for stego from a pixel count. Assumes that the attached AES header will be added.
        /// </summary>
        /// <param name="imgSize"> The pixel count of the image</param>
        /// <returns> The number of available bytes for stego</returns>
        public static int availableBytes(int imgSize) //img size in px i think?
        {
            //math on this is total px-2*stego header length all divided by 512 which is number of px for a byte of dispersed
            //-8 for EOF - AES.START_LENGTH for the header of the encryption. 
            return (((((imgSize * BYTES_IN_PX)/BITS_IN_BYTE)-StegoHandler.START_LENGTH)/STEGO_DENSITY) - (EOF1_LENGTH+1+2*BLOCK_LENGTH) - AES.START_LENGTH);
        }
    }
}
