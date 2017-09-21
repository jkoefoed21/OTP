using System;

namespace encryption
{
    /// <summary>
    /// Tests the encryption process ECB against a known test vector
    /// taken from http://www.inconteam.com/software-development/41-encryption/55-aes-test-vectors
    /// </summary>
    class AlgorithmTester
    {
        /// <summary>
        /// The gf28 table used in these encryptions.
        /// </summary>
        public static readonly GF28Table gfb = new GF28Table();

        /// <summary>
        /// The s-box used in these encryptions
        /// </summary>
        public static readonly sBox sb = new sBox(gfb);

        /// <summary>
        /// The key used for all tests.
        /// </summary>
        public static readonly byte[] key = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };

        /// <summary>
        /// The first test input.
        /// </summary>
        public static readonly byte[] test1 = { 0x6b, 0xc1, 0xbe, 0xe2, 0x2e, 0x40, 0x9f, 0x96, 0xe9, 0x3d, 0x7e, 0x11, 0x73, 0x93, 0x17, 0x2a };

        /// <summary>
        /// The expected first test output
        /// </summary>
        public static readonly byte[] result1 = { 0x3a, 0xd7, 0x7b, 0xb4, 0x0d, 0x7a, 0x36, 0x60, 0xa8, 0x9e, 0xca, 0xf3, 0x24, 0x66, 0xef, 0x97 };

        /// <summary>
        /// The 2nd test input
        /// </summary>
        public static readonly byte[] test2 = { 0xae, 0x2d, 0x8a, 0x57, 0x1e, 0x03, 0xac, 0x9c, 0x9e, 0xb7, 0x6f, 0xac, 0x45, 0xaf, 0x8e, 0x51 };

        /// <summary>
        /// The expected 2nd test output.
        /// </summary>
        public static readonly byte[] result2 = { 0xf5, 0xd3, 0xd5, 0x85, 0x03, 0xb9, 0x69, 0x9d, 0xe7, 0x85, 0x89, 0x5a, 0x96, 0xfd, 0xba, 0xaf };

        /// <summary>
        /// The main method. Program starts here.
        /// </summary>
        /// <param name="args"> Command line arguments </param>
        public static void Main(String[] args)
        {
            BitMatrix t1 = new BitMatrix(gfb, sb, test1, 0);
            BitMatrix f1 = new BitMatrix(gfb, sb, (byte[]) test1.Clone(), 0);
            BitMatrix r1 = new BitMatrix(gfb, sb, result1, 0);
            BitMatrix t2 = new BitMatrix(gfb, sb, test2, 0);
            BitMatrix f2 = new BitMatrix(gfb, sb, (byte[]) test2.Clone(), 0);
            BitMatrix r2 = new BitMatrix(gfb, sb, result2, 0);
            BitMatrix[] keySchedule = AES.getKeySchedule(key);
            AES.encryptSingle(keySchedule, t1);
            AES.encryptSingle(keySchedule, t2);

            Console.WriteLine(t1.Equals(r1) ? "Encrypt 1 Successful" : "Encrypt 1 Fail.");
            Console.WriteLine(t2.Equals(r2) ? "Encrypt 2 Successful" : "Encrypt 2 Fail.");

            AES.decryptSingle(keySchedule, t1);
            AES.decryptSingle(keySchedule, t2);

            Console.WriteLine(t1.Equals(f1) ? "Decrypt 1 Successful" : "Decrypt 1 Fail.");
            Console.WriteLine(t2.Equals(f2) ? "Decrypt 2 Successful" : "Decrypt 2 Fail.");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
