using System;
using System.Numerics;

public class NTT
{
    private const long MOD = 998244353;

    private const long G = 3;
    private static long Power(long a, long b, long mod)
    {
        long result = 1;
        a %= mod;
        while (b > 0)
        {
            if ((b & 1) == 1)
                result = result * a % mod;
            a = a * a % mod;
            b >>= 1;
        }
        return result;
    }

    private static long Inverse(long a, long mod)
    {
        return Power(a, mod - 2, mod);
    }

    public static void Transform(long[] a, bool invert)
    {
        int n = a.Length;

        for (int i = 1, j = 0; i < n; i++)
        {
            int bit = n >> 1;
            for (; (j & bit) != 0; bit >>= 1)
                j ^= bit;
            j ^= bit;

            if (i < j)
            {
                long temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            long wlen = Power(G, (MOD - 1) / len, MOD);
            if (invert)
                wlen = Inverse(wlen, MOD);

            for (int i = 0; i < n; i += len)
            {
                long w = 1;
                for (int j = 0; j < len / 2; j++)
                {
                    long u = a[i + j];
                    long v = a[i + j + len / 2] * w % MOD;
                    a[i + j] = (u + v) % MOD;
                    a[i + j + len / 2] = (u - v + MOD) % MOD;
                    w = w * wlen % MOD;
                }
            }
        }

        if (invert)
        {
            long nInv = Inverse(n, MOD);
            for (int i = 0; i < n; i++)
                a[i] = a[i] * nInv % MOD;
        }
    }

    public static long[] Multiply(long[] a, long[] b)
    {
        int resultLength = a.Length + b.Length - 1;
        int n = 1;
        while (n < resultLength)
            n <<= 1;

        long[] A = new long[n];
        long[] B = new long[n];

        for (int i = 0; i < a.Length; i++)
            A[i] = a[i] % MOD;
        for (int i = 0; i < b.Length; i++)
            B[i] = b[i] % MOD;

        Transform(A, false);
        Transform(B, false);

        for (int i = 0; i < n; i++)
            A[i] = A[i] * B[i] % MOD;

        Transform(A, true);

        long[] result = new long[resultLength];
        for (int i = 0; i < resultLength; i++)
            result[i] = A[i];

        return result;
    }
}