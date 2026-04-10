using System.Numerics;
using System.Security.Cryptography;

namespace Shared;

public class IdGenerator
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Generate()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(16);
        BigInteger value = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

        char[] buffer = new char[22];
        int i = buffer.Length;

        while (value > 0)
        {
            value = BigInteger.DivRem(value, 62, out var remainder);
            buffer[--i] = Alphabet[(int)remainder];
        }

        while (i > 0)
            buffer[--i] = '0';

        return new string(buffer);
    }
}