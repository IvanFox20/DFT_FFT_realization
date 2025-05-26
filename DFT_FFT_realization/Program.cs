using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

class Program
{
    static void Main()
    {
        BigPolynomial();
        Console.WriteLine();
        Console.WriteLine("Тест на небольшом многочлене");
        ShortPolynomial();
    }

    private static void ShortPolynomial()
    {
        // Пример входных данных
        double[] a = { 1, 2, 3 }; // A(x) = 1 + 2x + 3x^2
        double[] b = { 4, 5 };    // B(x) = 4 + 5x

        // Замер времени для наивного умножения
        var swNaive = Stopwatch.StartNew();
        double[] resultNaive = PolynomialMultiplication.MultiplyNaive(a, b);
        swNaive.Stop();

        // Замер времени для умножения через БПФ
        var swFFT = Stopwatch.StartNew();
        double[] resultFFT = PolynomialMultiplication.MultiplyUsingFFT(a, b);
        swFFT.Stop();

        Console.WriteLine("Тривиальное умножение:");
        Print(resultNaive);
        Console.WriteLine($"Время выполнения: {swNaive.Elapsed.TotalMilliseconds:F5} мс\n");

        Console.WriteLine("Умножение через БПФ:");
        Print(resultFFT);
        Console.WriteLine($"Время выполнения: {swFFT.Elapsed.TotalMilliseconds:F5} мс");
    }

    static void Print(double[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            Console.WriteLine($"x^{i}: {arr[i]}");
        }
        Console.WriteLine();
    }
    private static void BigPolynomial()
    {
        int size = 1 << 16; // 4096
        Random rand = new Random();

        // Генерация случайных коэффициентов
        double[] a = Enumerable.Repeat(0, size).Select(_ => rand.NextDouble()).ToArray();
        double[] b = Enumerable.Repeat(0, size).Select(_ => rand.NextDouble()).ToArray();

        Console.WriteLine($"Умножение многочленов длиной {size}...");

        // --- Наивное умножение ---
        var swNaive = Stopwatch.StartNew();
        double[] resultNaive = PolynomialMultiplication.MultiplyNaive(a, b);
        swNaive.Stop();

        Console.WriteLine($"Тривиальное умножение завершено за {swNaive.Elapsed.TotalSeconds:F3} сек.");

        // --- Умножение через БПФ ---
        var swFFT = Stopwatch.StartNew();
        double[] resultFFT = PolynomialMultiplication.MultiplyUsingFFT(a, b);
        swFFT.Stop();

        Console.WriteLine($"Умножение через БПФ завершено за {swFFT.Elapsed.TotalSeconds:F3} сек.");

        // --- Опционально: проверка совпадения первых N коэффициентов ---
        int checkCount = 5;
        Console.WriteLine("\nСравнение первых нескольких коэффициентов:");
        for (int i = 0; i < checkCount; i++)
        {
            Console.WriteLine($"x^{i}: Naive = {resultNaive[i]:F4}, FFT = {resultFFT[i]:F4}");
        }
    }
}