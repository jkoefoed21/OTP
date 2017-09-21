
namespace encryption
{
    /// <summary>
    /// A class containing the rijndael s-box and the inverse.
    /// </summary>
    class sBox
    {
        /// <summary>
        /// The size of the box.
        /// </summary>
        public static readonly int SIZE = 256;

        /// <summary>
        /// The forward box.
        /// </summary>
        private byte[] box;

        /// <summary>
        /// The inverse box.
        /// </summary>
        private byte[] invBox;

        /// <summary>
        /// Constructs both boxes.
        /// </summary>
        /// <param name="gfB"> The GF(2^8) table being used. </param>
        public sBox(GF28Table gfB)
        {
            box = new byte[SIZE];
            invBox = new byte[SIZE];
            for (int r = 0; r < SIZE; r++)
            {
                for (int ii = 0; ii < SIZE * SIZE; ii++) //for every possible mult. inv
                {
                    if (gfB.multiply((byte)(r), (byte)ii) == 1) //gets MI
                    {
                        byte x = (byte)ii;
                        byte y = (byte)ii;
                        for (int jj = 0; jj < 4; jj++) //this is the algorithm for the affine transformation.
                        {                              //it rotates and XORs 4 times. See https://en.wikipedia.org/wiki/Rijndael_S-box
                            y = bitRotate(y);//rotate the byte 4 times
                            x = (byte)(x ^ y); //mod the byte with the rotation
                        }
                        x = (byte)(x ^ 0x63); //to make nothing map to itself, 0x63 is contrived and in spec.
                        box[r] = x;
                        invBox[x] = (byte)(r); //puts r at location x in the inverse.
                        break;
                    }
                }
            } //0 maps to 0, and gets xored to 0x63, because 0 has no mult. inv.
            box[0] = 0x63; //these are because 0 won't pull an MI--could go and put 0=0 above
            invBox[0x63] = 0x00; //but I would need more for-loops and it would get messy
        }

        /// <summary>
        /// Gets the box in the S-box corresponding with the specified byte.
        /// </summary>
        /// <param name="b"> The byte inputted.</param>
        /// <returns> The corresponding byte in the output. </returns>
        public byte getByte(byte b)
        {
            return box[b];
        }

        /// <summary>
        /// Gets the box in the inverse S-box corresponding with the specified byte.
        /// </summary>
        /// <param name="b"> The byte inputted.</param>
        /// <returns> The corresponding byte in the output. </returns>
        public byte getInvByte(byte b)
        {
            return invBox[b];
        }

        /// <summary>
        /// Rotates a byte by one bit.
        /// </summary>
        /// <param name="y">The inputted byte.</param>
        /// <returns> The rotated byte. </returns>
        private static byte bitRotate(byte y)
        {
            return (byte) (y << 1 | y >> 7);
        }

        /// <summary>
        /// Generates a string representation of both boxes.
        /// </summary>
        /// <returns>The string representation. </returns>
        public override string ToString()
        {
            string output = "";
            for (int r = 0; r < box.GetLength(0); r++)
            {
                for (int c = 0; c < box.GetLength(1); c++)
                {
                    output += box[16*r+c].ToString("X") + "\t";
                }
                output += "\n";
            }
            output += "\n\n";
            for (int r = 0; r < invBox.GetLength(0); r++)
            {
                for (int c = 0; c < invBox.GetLength(1); c++)
                {
                    output += invBox[16*r+c].ToString("X") + "\t";
                }
                output += "\n";
            }
            return output;
        }
    }
}
