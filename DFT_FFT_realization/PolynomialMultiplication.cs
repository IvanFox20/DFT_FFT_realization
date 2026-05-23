using System;
using System.Numerics;
using System.Diagnostics;
public class PolynomialMultiplication
{
    // Используем ранее реализованный FFT.ComputeFFT
    public static double[] MultiplyUsingFFT(double[] a, double[] b)
    {
        int n = a.Length;
        int m = b.Length;
        int resultLength = n + m - 1;

        // Найдём ближайшую степень двойки >= resultLength
        int fftSize = GetNextPowerOfTwo(resultLength);

        // Расширяем массивы до fftSize и преобразуем в Complex
        Complex[] A = new Complex[fftSize];
        Complex[] B = new Complex[fftSize];

        for (int i = 0; i < fftSize; i++)
        {
            A[i] = i < n ? new Complex(a[i], 0) : Complex.Zero;
            B[i] = i < m ? new Complex(b[i], 0) : Complex.Zero;
        }

        // Прямое БПФ
        FFT.ComputeFFT(A);
        FFT.ComputeFFT(B);

        // Поэлементное умножение в частотной области
        Complex[] C = new Complex[fftSize];
        for (int i = 0; i < fftSize; i++)
        {
            C[i] = A[i] * B[i];
        }

        // Обратное БПФ
        FFT.ComputeIFFT(C);

        // Нормировка (обратное БПФ требует деления на размер)
        double[] result = new double[resultLength];
        for (int i = 0; i < resultLength; i++)
        {
            result[i] = C[i].Real;
        }

        return result;
    }

    // Получает ближайшую степень двойки, большую или равную x
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