using System;
using System.Numerics;
using System.Diagnostics;
public class PolynomialMultiplication
{
    public static double[] MultiplyUsingFFT(double[] a, double[] b)
    {
        int n = a.Length;
        int m = b.Length;
        int resultLength = n + m - 1;

        int fftSize = GetNextPowerOfTwo(resultLength);

        Complex[] A = new Complex[fftSize];
        Complex[] B = new Complex[fftSize];

        for (int i = 0; i < fftSize; i++)
        {
            A[i] = i < n ? new Complex(a[i], 0) : Complex.Zero;
            B[i] = i < m ? new Complex(b[i], 0) : Complex.Zero;
        }

        FFT.ComputeFFT(A);
        FFT.ComputeFFT(B);

        Complex[] C = new Complex[fftSize];
        for (int i = 0; i < fftSize; i++)
        {
            C[i] = A[i] * B[i];
        }

        FFT.ComputeIFFT(C);

        double[] result = new double[resultLength];
        for (int i = 0; i < resultLength; i++)
        {
            result[i] = Math.Round(C[i].Real);
        }

        return result;
    }

    private static int GetNextPowerOfTwo(int x)
    {
        if (x == 0)
            return 1;
        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        x++;
        return x;
    }

    public static double[] MultiplyNaive(double[] a, double[] b)
    {
        int n = a.Length;
        int m = b.Length;
        int resultLength = n + m - 1;

        double[] result = new double[resultLength];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                result[i + j] += a[i] * b[j];
            }
        }

        return result;
    }

}