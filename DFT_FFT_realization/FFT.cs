using System;
using System.Numerics;

public class FFT
{
    // Проверяет, является ли число степенью двойки
    public static bool IsPowerOfTwo(int x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }

    // Возвращает количество бит для заданного размера
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

    // Обращает порядок битов числа
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

    // Нерекурсивный алгоритм БПФ
    public static void ComputeFFT(Complex[] data)
    {
        int N = data.Length;

        if (!IsPowerOfTwo(N))
            throw new ArgumentException("Длина массива должна быть степенью двойки.");

        int bitLength = BitLength(N);

        // Бит-реверсивная перестановка
        for (int i = 0; i < N; i++)
        {
            int j = ReverseBits(i, bitLength);
            if (i < j)
                (data[i], data[j]) = (data[j], data[i]);
        }

        // Вычисление "бабочек"
        for (int s = 1; s <= bitLength; s++)
        {
            int m = 1 << s;             // Размер текущего уровня (2^s)
            double theta = -2 * Math.PI / m;
            Complex Wm = Complex.FromPolarCoordinates(1, theta);

            for (int k = 0; k < N; k += m)
            {
                Complex W = Complex.One;
                for (int j = 0; j < m / 2; j++)
                {
                    Complex t = W * data[k + j + m / 2];  // Twiddle factor * нечетное значение
                    Complex u = data[k + j];              // Четное значение

                    data[k + j] = u + t;                  // Верхняя часть "бабочки"
                    data[k + j + m / 2] = u - t;          // Нижняя часть "бабочки"

                    W *= Wm;                              // Следующий поворотный множитель
                }
            }
        }
    }
    // В классе FFT
    public static void ComputeIFFT(Complex[] data)
    {
        int N = data.Length;

        if (!IsPowerOfTwo(N))
            throw new ArgumentException("Длина массива должна быть степенью двойки.");

        int bitLength = BitLength(N);

        // Бит-реверсивная перестановка
        for (int i = 0; i < N; i++)
        {
            int j = ReverseBits(i, bitLength);
            if (i < j)
                (data[i], data[j]) = (data[j], data[i]);
        }

        // Вычисление "бабочек"
        for (int s = 1; s <= bitLength; s++)
        {
            int m = 1 << s;             // Размер текущего уровня (2^s)
            double theta = 2 * Math.PI / m;  // Обратное преобразование: положительный угол
            Complex Wm = Complex.FromPolarCoordinates(1, theta);

            for (int k = 0; k < N; k += m)
            {
                Complex W = Complex.One;
                for (int j = 0; j < m / 2; j++)
                {
                    Complex t = W * data[k + j + m / 2];  // Twiddle factor * нечетное значение
                    Complex u = data[k + j];              // Четное значение

                    data[k + j] = u + t;                  // Верхняя часть "бабочки"
                    data[k + j + m / 2] = u - t;          // Нижняя часть "бабочки"

                    W *= Wm;                              // Следующий поворотный множитель
                }
            }
        }
    }
}