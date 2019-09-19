using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
static class Md4
{
    public static string Md4Hash(string input)
    {
        byte[] bytesRead = null;

        try
        {
            // nusiskaitom bytes is failo
            bytesRead = File.ReadAllBytes(input);
        }
        catch (Exception e)
        {
            string nope = "No such file!";
            return nope;
        }
        List<byte> bytes = new List<byte>();
        // persikeliam bytes i nauja lista
        foreach (var by in bytesRead)
        {
            bytes.Add(by);
        }

        uint bitCount = (uint)(bytes.Count) * 8; // pasiverciam bit'ais

        bytes.Add(128);
        while (bytes.Count % 64 != 56)// 512/8 = 64 ir 448/8 = 56 pridejinejam nulius
        {
            bytes.Add(0);
        }

        var uints = new List<uint>();
        for (int i = 0; i + 3 < bytes.Count; i += 4) // shiftinam i kaire
        {
            uints.Add(bytes[i] | (uint)bytes[i + 1] << 8 | (uint)bytes[i + 2] << 16 | (uint)bytes[i + 3] << 24); // kopijuoja 512 bloka kaip 16x32bit zodzius
        }

        uints.Add(bitCount);
        uints.Add(0);

        // apsibreziam nurodytas MD4 reiksmes
              uint A = 0x67452301;
              uint B = 0xefcdab89;
              uint C = 0x98badcfe;
              uint D = 0x10325476;

        // apsirasom funkcijas
              Func<uint, uint, uint, uint> F = (x, y, z) => (x & y) | (~x & z);
              Func<uint, uint, uint, uint> G = (x, y, z) => (x & y) | (x & z) | (y & z);
              Func<uint, uint, uint, uint> H = (x, y, z) => x ^ y ^ z;

        // apsirasom left rotate funkcija
              Func<uint, uint, uint> leftRotate = (x, y) => x << (int)y | x >> 32 - (int)y;

        // round1 - 1,11,12,13 kad galetume keist visu 4 registru reiksmes
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND1 = (a, b, c, d, x, m) => A = leftRotate((a + F(b, c, d) + x), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND11 = (d, a, b, c, x, m) => D = leftRotate((d + F(a, b, c) + x), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND12 = (c, d, a, b, x, m) => C = leftRotate((c + F(d, a, b) + x), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND13 = (b, c, d, a, x, m) => B = leftRotate((b + F(c, d, a) + x), m);
        // Round2
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND2 = (a, b, c, d, x, m) => A = leftRotate((a + G(b, c, d) + x + (uint)0x5a827999), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND21 = (d, a, b, c, x, m) => D = leftRotate((d + G(a, b, c) + x + (uint)0x5a827999), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND22 = (c, d, a, b, x, m) => C = leftRotate((c + G(d, a, b) + x + (uint)0x5a827999), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND23 = (b, c, d, a, x, m) => B = leftRotate((b + G(c, d, a) + x + (uint)0x5a827999), m);
        //Round3
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND3 = (a, b, c, d, x, m) => A = leftRotate((a + H(b, c, d) + x + (uint)0x6ed9eba1), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND31 = (d, a, b, c, x, m) => D = leftRotate((d + H(a, b, c) + x + (uint)0x6ed9eba1), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND32 = (c, d, a, b, x, m) => C = leftRotate((c + H(d, a, b) + x + (uint)0x6ed9eba1), m);
              Func<uint, uint, uint, uint, uint, ushort, uint> ROUND33 = (b, c, d, a, x, m) => B = leftRotate((b + H(c, d, a) + x + (uint)0x6ed9eba1), m);

        // skirstom po 16 32bit zodziu
        for (int i = 0; i < uints.Count() / 16; i++)
        {
            uint[] X = new uint[16];
            for (int j = 0; j < 16; j++) // kopijuojame ta bloka i X
            {
                X[j] = uints[i * 16 + j];
            }
            // issisaugom turimas registru reiksmes, kad galetume panaudoti pabaigoje
                  uint AA = A;
                  uint BB = B;
                  uint CC = C;
                  uint DD = D;
            // duodam parametrus i aprasytus round funkcijas
                  ROUND1(A, B, C, D, X[0], 3);
                  ROUND11(D, A, B, C, X[1], 7);
                  ROUND12(C, D, A, B, X[2], 11);
                  ROUND13(B, C, D, A, X[3], 19);
                  ROUND1(A, B, C, D, X[4], 3);
                  ROUND11(D, A, B, C, X[5], 7);
                  ROUND12(C, D, A, B, X[6], 11);
                  ROUND13(B, C, D, A, X[7], 19);
                  ROUND1(A, B, C, D, X[8], 3);
                  ROUND11(D, A, B, C, X[9], 7);
                  ROUND12(C, D, A, B, X[10], 11);
                  ROUND13(B, C, D, A, X[11], 19);
                  ROUND1(A, B, C, D, X[12], 3);
                  ROUND11(D, A, B, C, X[13], 7);
                  ROUND12(C, D, A, B, X[14], 11);
                  ROUND13(B, C, D, A, X[15], 19);

                  ROUND2(A, B, C, D, X[0], 3);
                  ROUND21(D, A, B, C, X[4], 5);
                  ROUND22(C, D, A, B, X[8], 9);
                  ROUND23(B, C, D, A, X[12], 13);
                  ROUND2(A, B, C, D, X[1], 3);
                  ROUND21(D, A, B, C, X[5], 5);
                  ROUND22(C, D, A, B, X[9], 9);
                  ROUND23(B, C, D, A, X[13], 13);
                  ROUND2(A, B, C, D, X[2], 3);
                  ROUND21(D, A, B, C, X[6], 5);
                  ROUND22(C, D, A, B, X[10], 9);
                  ROUND23(B, C, D, A, X[14], 13);
                  ROUND2(A, B, C, D, X[3], 3);
                  ROUND21(D, A, B, C, X[7], 5);
                  ROUND22(C, D, A, B, X[11], 9);
                  ROUND23(B, C, D, A, X[15], 13);

                  ROUND3(A, B, C, D, X[0], 3);
                  ROUND31(D, A, B, C, X[8], 9);
                  ROUND32(C, D, A, B, X[4], 11);
                  ROUND33(B, C, D, A, X[12], 15);
                  ROUND3(A, B, C, D, X[2], 3);
                  ROUND31(D, A, B, C, X[10], 9);
                  ROUND32(C, D, A, B, X[6], 11);
                  ROUND33(B, C, D, A, X[14], 15);
                  ROUND3(A, B, C, D, X[1], 3);
                  ROUND31(D, A, B, C, X[9], 9);
                  ROUND32(C, D, A, B, X[5], 11);
                  ROUND33(B, C, D, A, X[13], 15);
                  ROUND3(A, B, C, D, X[3], 3);
                  ROUND31(D, A, B, C, X[11], 9);
                  ROUND32(C, D, A, B, X[7], 11);
                  ROUND33(B, C, D, A, X[15], 15);

                  A += AA;
                  B += BB;
                  C += CC;
                  D += DD;
              }

        // grazinam hex encodinta string
        byte[] exportBytes = new[] { A, B, C, D }.SelectMany(BitConverter.GetBytes).ToArray();
        return BitConverter.ToString(exportBytes).Replace("-", "").ToLower();
    }

    static void Main()
    {
        string input = Console.ReadLine(); //C:\Users\antanas\Desktop\Antanas_Katilauskas_Planas.pdf
        Console.WriteLine(Md4Hash(input));
        Console.ReadLine();
    }
}