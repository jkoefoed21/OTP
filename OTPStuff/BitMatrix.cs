using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encryption
{
    /// <summary>
    /// A class used to manipulate each block.
    /// To work on---a decrypt version of shift and mix.
    /// </summary>
    class BitMatrix
    {

        /// <summary>
        /// The length of a word, in bytes.
        /// </summary>
        public static readonly int WORD_LENGTH = 4;

        /// <summary>
        /// The length of a 128 bit AES block
        /// </summary>
        public static readonly int SIZE = 16;

        /// <summary>
        /// Used for shift and mix as a helper table to compute locations before and after
        /// the shift rows operation.
        /// </summary>
        public static readonly int[] shiftMixTable = instantiateShiftMixTable();

        /// <summary>
        /// Used for shift and mix as a helper table to compute locations before and after
        /// the shift rows operation.
        /// </summary>
        public static readonly int[] invShiftMixTable = instantiateInvShiftMixTable();

        /// <summary>
        /// The S-Box used for rijndael operations.
        /// </summary>
        public readonly sBox subBox; //box for the sub Bytes step

        /// <summary>
        /// The GF(2^8) table used for the mix columns step.
        /// </summary>
        public readonly GF28Table GFBox;

        /// <summary>
        /// The first index of the underlying byte array operated on in this object
        /// </summary>
        public readonly int startValue;

        /// <summary>
        /// The bytes contained in the matrix.
        /// </summary>
        private byte[] bytes;

        /// <summary>
        /// Constructs a matrix containing all values of 0x00.
        /// </summary>
        /// <param name="g"> The GF(2^8) field being used. </param>
        /// <param name="s"> The S-Box being used. </param>
        /// <param name="b"> The underlying byte array </param>
        /// <param name="sv"> The index of the first value contained in this object within b. </param>
        public BitMatrix(GF28Table g, sBox s, byte[] b, int sv)
        {
            subBox = s;
            GFBox = g;
            bytes = b;
            startValue = sv;
        }

        /// <summary>
        /// Constructs a new BitMatrix with the specified 4 "words". 
        /// </summary>
        /// <param name="g"> The GF(2^8) field being used. </param>
        /// <param name="s"> The S-Box being used. </param>
        /// <param name="a">The first word. </param>
        /// <param name="b"> The second word. </param>
        /// <param name="c"> The third word. </param>
        /// <param name="d"> THe fourth word. </param>
        public BitMatrix(GF28Table g, sBox s, byte[] a, byte[] b, byte[] c, byte[] d)
        {
            bytes = new byte[SIZE];
            if (a.Length != WORD_LENGTH || b.Length != WORD_LENGTH || c.Length != WORD_LENGTH || d.Length != WORD_LENGTH)
            {
                throw new ArgumentException();
            }
            Array.Copy(a, 0, bytes, 0, a.Length);
            Array.Copy(b, 0, bytes, WORD_LENGTH, b.Length);
            Array.Copy(c, 0, bytes, 2*WORD_LENGTH, c.Length);
            Array.Copy(d, 0, bytes, 3*WORD_LENGTH, d.Length);
            startValue = 0;
        }

        /// <summary>
        /// Gets the underlying array of this BitMatrix.
        /// </summary>
        /// <returns> The array. </returns>
        public byte[] getBytes()
        {
            return bytes;
        }

        /// <summary>
        /// XORs each byte of this BitMatrix with the corresponding byte in the inputted BitMatrix.
        /// </summary>
        /// <param name="other"> The BitMatrix containing the key. </param>
        public void addRoundKey(BitMatrix other)
        {
            for (int ii = 0; ii < SIZE; ii++)
            {
                bytes[startValue + ii] = (byte)(bytes[ii + startValue] ^ other.bytes[ii + other.startValue]);
            }
        }

        /// <summary>
        /// Replaces each value in this BitMatrix with the corresponding value in the S-Box.
        /// </summary>
        public void subBytes()
        {
            for (int ii = 0; ii < SIZE; ii++)
            {
                bytes[startValue + ii] = subBox.getByte(bytes[startValue + ii]);
            }
        }

        /// <summary>
        /// Replaces each value in this BitMatrix with the corresponding value in the inverse S-Box.
        /// </summary>
        public void invSubBytes()
        {
            for (int ii = 0; ii < SIZE; ii++)
            {
                bytes[startValue + ii] = subBox.getInvByte(bytes[startValue + ii]);
            }
        }

        /// <summary>
        /// Conducts the mixColumns step of the AES algorithm. Now Deprecated
        /// </summary>
        public void mixColumns() 
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii = 0; ii < newBytes.Length; ii++)
            {   //2, 3, 1, 1 are hard-coded into the algorithm for the GF(2^8) multiplication here.
                newBytes[ii] = (byte)(GFBox.multiply(2, bytes[startValue + ii]) ^ 
                                      GFBox.multiply(3, bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 1) % WORD_LENGTH]) ^
                                      bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 2) % WORD_LENGTH] ^ 
                                      bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 3) % WORD_LENGTH]);
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }

        /// <summary>
        /// Conducts the inverse mix columns step of the AES algorithm. Now Deprecated
        /// </summary>
        public void invMixColumns()
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii = 0; ii < newBytes.Length; ii++)
            {       //14, 11, 13, and 9 are hard coded as inverses to 2, 3, 1, 1 in the forward Mix Columns
                newBytes[ii] = (byte)(GFBox.multiply(14, bytes[startValue + ii]) ^ 
                                      GFBox.multiply(11, bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 1) % WORD_LENGTH]) ^
                                      GFBox.multiply(13, bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 2) % WORD_LENGTH]) ^ 
                                      GFBox.multiply(9, bytes[startValue + WORD_LENGTH * (ii / WORD_LENGTH) + (ii + 3) % WORD_LENGTH]));
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }

        /// <summary>
        /// Conducts shiftRows and MixColumns with only a single array copy instead of two. Approx 10% faster.
        /// Basically performs mixcolumns, but on values specified in the SMtable which is a map of what SR does.
        /// </summary>
        public void shiftAndMix()
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii=0; ii<newBytes.Length; ii++)
            {
                  newBytes[ii] = (byte)(GFBox.multiply(2, bytes[startValue + shiftMixTable[4*ii]]) ^
                  GFBox.multiply(3, bytes[startValue + shiftMixTable[4*ii+1]]) ^
                  bytes[startValue + shiftMixTable[4*ii+2]] ^
                  bytes[startValue + shiftMixTable[4*ii+3]]);
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }
                      
        ///<summary>
        /// Acts as a helper method to shift and mix, by creating a table that does a bunch of messy arithmetic and
        /// Makes each one into a 2D array table lookup. Feeds out a 64 but functionally a 
        /// 16x4--4 values for each of the 16 spots in the matrix
        /// </summary>                                            
        private static int[] instantiateShiftMixTable()
        {
            int[][] smTab = new int[16][];
            for (int ii=0; ii<SIZE; ii++)
            {
                smTab[ii] = new int[4] { ((5 * (ii % WORD_LENGTH) + WORD_LENGTH * (ii / WORD_LENGTH)) % 16),
                        ((5 * ((ii + 1) % WORD_LENGTH) + WORD_LENGTH * (ii / WORD_LENGTH)) % 16), 
                        (5 * ((ii + 2) % WORD_LENGTH) + WORD_LENGTH * (ii / WORD_LENGTH)) % 16,
                        (5 * ((ii + 3) % WORD_LENGTH) + WORD_LENGTH * (ii / WORD_LENGTH)) % 16 };
            }
            int[] table = new int[64];
            for (int ii=0; ii<smTab.Length; ii++)
            {
                table[4 * ii] = smTab[ii][0];
                table[4 * ii+1] = smTab[ii][1];
                table[4 * ii+2] = smTab[ii][2];
                table[4 * ii+3] = smTab[ii][3];
            }
            return table;
        }

        /// <summary>
        /// Conducts invMixColumns and Shift Rows with only a single array copy instead of two. Much faster faster.
        /// Basically performs mixcolumns, but on values specified in the iSMtable which is a map of what SR does.
        /// </summary>
        public void invShiftAndMix()
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii = 0; ii < newBytes.Length; ii++)
            {
                newBytes[ii] = (byte)(GFBox.multiply(14, bytes[startValue + invShiftMixTable[4 * ii]]) ^
                GFBox.multiply(11, bytes[startValue + invShiftMixTable[4 * ii + 1]]) ^
                GFBox.multiply(13, bytes[startValue + invShiftMixTable[4 * ii + 2]]) ^
                GFBox.multiply(9, bytes[startValue + invShiftMixTable[4 * ii + 3]]));
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }

        /// <summary>
        /// Instantiates the table for invShiftandMix--different architecture from shiftandmix because mix is first
        /// Much faster than the two seperate.
        /// </summary>
        /// <returns> The Inv Shift Mix table </returns>
        private static int[] instantiateInvShiftMixTable()
        {
            int[][] smTab = new int[16][];
            for (int ii = 0; ii < SIZE; ii++)
            {
                int root = ((((SIZE - 3 * (ii % 4) + 4 * (ii / 4)) % SIZE) / 4) * 4);
                //this could be optimized, but it happens once a run, so it doesn't really matter
                smTab[ii] = new int[4] { (root+((16 - 3 * (ii % 4)) % 4)),
                                         (root+((17 - 3 * (ii % 4)) % 4)),
                                         (root+((18 - 3 * (ii % 4)) % 4)),
                                         (root+((19 - 3 * (ii % 4)) % 4)) };
            }
            //creates as 2D, parses to 1D
            int[] table = new int[64];
            for (int ii = 0; ii < smTab.Length; ii++)
            {
                table[4 * ii] = smTab[ii][0];
                table[4 * ii + 1] = smTab[ii][1];
                table[4 * ii + 2] = smTab[ii][2];
                table[4 * ii + 3] = smTab[ii][3];
            }
            return table;
        }

        /// <summary>
        /// Conducts the shift rows step of the AES algorithm.
        /// </summary>
        public void shiftRows()
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii = 0; ii < newBytes.Length; ii++)
            {
                newBytes[ii] = bytes[startValue + ((ii + (WORD_LENGTH * (ii % WORD_LENGTH))) % SIZE)];
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }

        /// <summary>
        /// Conducts the inverse shift rows step of the AES algorithm. 
        /// </summary>
        public void invShiftRows()
        {
            byte[] newBytes = new byte[SIZE];
            for (int ii = 0; ii < newBytes.Length; ii++)
            {
                newBytes[ii] = bytes[startValue + ((SIZE + ii - (WORD_LENGTH * (ii % WORD_LENGTH))) % SIZE)];
            }
            Array.Copy(newBytes, 0, bytes, startValue, SIZE);
        }

        /// <summary>
        /// Gets a word from this BitMatrix.
        /// </summary>
        /// <param name="c"> The index of the specified word. </param>
        /// <returns>The word as a byte array.</returns>
        public byte[] getWord(int c)
        {
            if (c < 0 || c > 3) //words are 0-3.
            {
                throw new ArgumentException();
            }
            byte[] b = { bytes[startValue + WORD_LENGTH * c], bytes[startValue + WORD_LENGTH * c + 1],
                         bytes[startValue + WORD_LENGTH * c + 2], bytes[startValue + WORD_LENGTH * c + 3] };
            return b;
        }

        /// <summary>
        /// Gets the 0-WORD_LENGTH*WORD_LENGTH indexed byte from this BitMatrix.
        /// </summary>
        /// <param name="n"> THe index of the byte being sought. </param>
        /// <returns> The byte. </returns>
        public byte getByte(int n)
        {
            return bytes[n + startValue];
        }

        /// <summary>
        /// Provides a string representation of this object.
        /// </summary>
        /// <returns> A string representation. </returns>
        public override string ToString() //provides a grid representation.
        {
            string output = "";
            for (int ii = 0; ii < SIZE; ii++)
            {
                output += bytes[ii + startValue].ToString("X") + " ";
            }
            return output;
        }

        /// <summary>
        /// Checks if this object is equal to another.
        /// </summary>
        /// <param name="obj"> The object to be checked</param>
        /// <returns> True if this is equal to another bitMatrix</returns>
        public override bool Equals(object obj)
        {
            if (obj==null||!(obj is BitMatrix))
            {
                return false;
            }
            BitMatrix b = (BitMatrix)obj;
            for (int ii = 0; ii < SIZE; ii++)
            {
                if (b.bytes[b.startValue+ii] != this.bytes[startValue+ii])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Serves as the hash function for this object.
        /// </summary>
        /// <returns> The hash value of this object </returns>
        public override int GetHashCode()
        {
            int result = 0;
            for (int ii=0; ii<SIZE; ii++)
            {
                result += (int) Math.Pow(bytes[startValue + ii], ii+1);
            }
            return result;
        }
    }
}
