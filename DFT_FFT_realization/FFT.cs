using System;
using System.Numerics;

public class FFT
{
    public static bool IsPowerOfTwo(int x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }

    private static int BitLength(int n)
    {
        int bits = 0;
        while (n > 1)
        {
            n >>= 1;
            bits++;
        }
        return bits;
    }

    private static int ReverseBits(int n, int bitLength)
    {
        int reversed = 0;
        for (int i = 0; i < bitLength; i++)
        {
            reversed <<= 1;
            reversed |= (n >> i) & 1;
        }
        return reversed;
    }

    public static void ComputeFFT(Complex[] data)
    {
        int N = data.Length;

        if (!IsPowerOfTwo(N))
            throw new ArgumentException("Длина массива должна быть степенью двойки.");

        int bitLength = BitLength(N);

        for (int i = 0; i < N; i++)
        {
            int j = ReverseBits(i, bitLength);
            if (i < j)
                (data[i], data[j]) = (data[j], data[i]);
        }

        for (int s = 1; s <= bitLength; s++)
        {
            int m = 1 << s;
            double theta = -2 * Math.PI / m;
            Complex Wm = Complex.FromPolarCoordinates(1, theta);

            for (int k = 0; k < N; k += m)
            {
                Complex W = Complex.One;
                for (int j = 0; j < m / 2; j++)
                {
                    Complex t = W * data[k + j + m / 2]; 
                    Complex u = data[k + j];

                    data[k + j] = u + t;
                    data[k + j + m / 2] = u - t;

                    W *= Wm;
                }
            }
        }
    }
    public static void ComputeIFFT(Complex[] data)
    {
        int N = data.Length;

        if (!IsPowerOfTwo(N))
            throw new ArgumentException("Длина массива должна быть степенью двойки.");

        int bitLength = BitLength(N);

        for (int i = 0; i < N; i++)
        {
            int j = ReverseBits(i, bitLength);
            if (i < j)
                (data[i], data[j]) = (data[j], data[i]);
        }

        for (int s = 1; s <= bitLength; s++)
        {
            int m = 1 << s;
            double theta = 2 * Math.PI / m;
            Complex Wm = Complex.FromPolarCoordinates(1, theta);

            for (int k = 0; k < N; k += m)
            {
                Complex W = Complex.One;
                for (int j = 0; j < m / 2; j++)
                {
                    Complex t = W * data[k + j + m / 2];
                    Complex u = data[k + j];

                    data[k + j] = u + t;
                    data[k + j + m / 2] = u - t;

                    W *= Wm;
                }
            }
        }

        for (int i = 0; i < N; i++)
        {
            data[i] /= N;
        }
    }
}