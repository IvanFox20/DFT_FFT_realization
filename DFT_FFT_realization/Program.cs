using DFT_FFT_realization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

class Program
{
    static void Main()
    {
        CheckFFTCorrectness();

        AudioSpectrumAnalyzer.AnalyzeSignal();
        PerformanceGraph.BuildPerformanceGraph();

        BigPolynomial(power: 14);
        BigPolynomial(power: 15);
        BigPolynomial(power: 16);
        Console.WriteLine();
        Console.WriteLine("Тест на небольшом многочлене");
        ShortPolynomial();
        ShopRobberTask();
    }

    private static void ShopRobberTask()
    {
        Console.WriteLine("Выберите режим работы:");
        Console.WriteLine("1 - Ручной ввод данных");
        Console.WriteLine("2 - Случайная генерация данных");

        string mode = Console.ReadLine();
        int n = 0, k = 0;
        int[] costs = null;

        if (mode == "1")
        {
            // Ручной ввод данных
            Console.WriteLine("Введите количество типов товаров (n) и количество товаров (k):");
            string[] input = Console.ReadLine().Split();
            n = int.Parse(input[0]);
            k = int.Parse(input[1]);

            Console.WriteLine("Введите стоимости товаров через пробел:");
            string[] costsInput = Console.ReadLine().Split();
            costs = Array.ConvertAll(costsInput, int.Parse);
        }
        else if (mode == "2")
        {
            while (true)
            {
                Console.WriteLine("Введите количество типов товаров (n) и количество товаров (k):");
                string line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("❌ Ошибка: пустой ввод. Введите два числа через пробел.");
                    continue;
                }

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.WriteLine("❌ Ошибка: нужно ввести ровно два числа.");
                    continue;
                }

                if (int.TryParse(parts[0], out int nVal) && int.TryParse(parts[1], out int kVal))
                {
                    if (nVal <= 0 || kVal <= 0)
                    {
                        Console.WriteLine("❌ Ошибка: числа должны быть больше нуля.");
                        continue;
                    }

                    n = nVal;
                    k = kVal;
                    break;
                }
                else
                {
                    Console.WriteLine("❌ Ошибка: введены нецелые числа или нечисловые символы.");
                }
            }

            costs = RandomGenerator.GenerateRandomCosts(n);
            Console.WriteLine("Сгенерированные стоимости товаров: " + string.Join(" ", costs));
        }
        else
        {
            Console.WriteLine("Неверный режим.");
            return;
        }

        // Решение задачи
        List<int> possibleSums = ShopRobber.Solve(n, k, costs);

        // Вывод результата
        Console.WriteLine("Возможные суммы стоимостей:");
        Console.WriteLine(string.Join(" ", possibleSums));
    }

    public static class RandomGenerator
    {
        private static readonly Random random = new Random();

        // Метод для генерации случайного массива стоимостей товаров
        public static int[] GenerateRandomCosts(int n, int minValue = 1, int maxValue = 1000)
        {
            int[] costs = new int[n];
            for (int i = 0; i < n; i++)
            {
                costs[i] = random.Next(minValue, maxValue + 1);
            }
            return costs;
        }
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
    private static void BigPolynomial(int power)
    {
        int size = 1 << power;
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

    private static void CheckFFTCorrectness()
    {
        Console.WriteLine("Проверка корректности FFT/IFFT");

        int size = 16;
        Random rand = new Random();

        // Генерация случайного массива
        Complex[] original = new Complex[size];

        for (int i = 0; i < size; i++)
        {
            original[i] = new Complex(rand.NextDouble(), rand.NextDouble());
        }

        // Копия массива
        Complex[] transformed = new Complex[size];
        Array.Copy(original, transformed, size);

        // Прямое БПФ
        FFT.ComputeFFT(transformed);

        // Обратное БПФ
        FFT.ComputeIFFT(transformed);

        // Вычисление максимальной ошибки
        double maxError = 0;

        for (int i = 0; i < size; i++)
        {
            double error = Complex.Abs(original[i] - transformed[i]);

            if (error > maxError)
            {
                maxError = error;
            }
        }

        Console.WriteLine($"Максимальная ошибка восстановления: {maxError:E6}");

        if (maxError < 1e-9)
        {
            Console.WriteLine("FFT/IFFT работают корректно.\n");
        }
        else
        {
            Console.WriteLine("Обнаружена значительная ошибка.\n");
        }
    }
}