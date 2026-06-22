using DFT_FFT_realization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  1. Проверка корректности FFT/IFFT                     ║");
            Console.WriteLine("║  2. Демонстрация умножения многочленов                 ║");
            Console.WriteLine("║  3. Задача «Вор в магазине»                            ║");
            Console.WriteLine("║  4. Спектральный анализ аудиосигнала                   ║");
            Console.WriteLine("║  5. Построение графика производительности              ║");
            Console.WriteLine("║  6. Запустить все демонстрации последовательно         ║");
            Console.WriteLine("║  0. Выход                                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.Write("\nВыберите пункт меню: ");

            string choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        CheckFFTCorrectness();
                        break;
                    case "2":
                        DemoPolynomialMultiplication();
                        break;
                    case "3":
                        ShopRobberTask();
                        break;
                    case "4":
                        AudioSpectrumAnalyzer.AnalyzeSignal();
                        break;
                    case "5":
                        PerformanceGraph.BuildPerformanceGraph();
                        break;
                    case "6":
                        RunAllDemos();
                        break;
                    case "0":
                        Console.WriteLine("\nЗавершение работы. Спасибо за внимание!");
                        return;
                    default:
                        Console.WriteLine("\nНеверный выбор. Попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ОШИБКА] {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }

    private static void RunAllDemos()
    {
        Console.WriteLine("\n=== Запуск всех демонстраций ===\n");

        CheckFFTCorrectness();
        Console.WriteLine("\n" + new string('-', 60) + "\n");

        DemoPolynomialMultiplication();
        Console.WriteLine("\n" + new string('-', 60) + "\n");

        ShopRobberTask();
        Console.WriteLine("\n" + new string('-', 60) + "\n");

        AudioSpectrumAnalyzer.AnalyzeSignal();
    }

    private static void DemoPolynomialMultiplication()
    {
        Console.WriteLine("\n=== Демонстрация умножения многочленов ===\n");

        int[] sizes = { 16, 32, 64, 128, 512, 2048, 4096, 16384, 32768,65536 };
        Random rnd = new Random(42);

        double[] warmup = new double[16];
        PolynomialMultiplication.MultiplyNaive(warmup, warmup);
        PolynomialMultiplication.MultiplyUsingFFT(warmup, warmup);

        Console.WriteLine("┌──────────┬────────────────┬────────────────┬────────────┐");
        Console.WriteLine("│ Размер N │ Классический   │ БПФ            │ Ускорение  │");
        Console.WriteLine("│          │ (мс)           │ (мс)           │            │");
        Console.WriteLine("├──────────┼────────────────┼────────────────┼────────────┤");

        foreach (int size in sizes)
        {
            double[] a = new double[size];
            double[] b = new double[size];

            for (int i = 0; i < size; i++)
            {
                a[i] = rnd.Next(10);
                b[i] = rnd.Next(10);
            }

            int iterations = size <= 2048 ? 50 : 1;

            var sw1 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                PolynomialMultiplication.MultiplyNaive(a, b);
            }
            sw1.Stop();
            double timeNaive = sw1.Elapsed.TotalMilliseconds / iterations;

            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                PolynomialMultiplication.MultiplyUsingFFT(a, b);
            }
            sw2.Stop();
            double timeFFT = sw2.Elapsed.TotalMilliseconds / iterations;

            string speedupStr = timeFFT > 0.001 ? $"{timeNaive / timeFFT:F1}x" : "-";

            Console.WriteLine($"│ {size,8} │ {timeNaive,14:F3} │ {timeFFT,14:F3} │ {speedupStr,10} │");
        }

        Console.WriteLine("└──────────┴────────────────┴────────────────┴────────────┘");
    }

    private static void ShopRobberTask()
    {
        Console.WriteLine("\n=== Задача «Вор в магазине» ===\n");
        Console.WriteLine("Выберите режим работы:");
        Console.WriteLine("  1 - Ручной ввод данных");
        Console.WriteLine("  2 - Случайная генерация данных");
        Console.WriteLine("  3 - Демонстрационный пример (a=[1,3,4], k=3)");
        Console.Write("\nВаш выбор: ");

        string mode = Console.ReadLine()?.Trim();
        int n = 0, k = 0;
        int[] costs = null;

        if (mode == "1")
        {
            // Ручной ввод данных
            Console.Write("\nВведите количество типов товаров (n) и количество товаров (k): ");
            string[] input = Console.ReadLine().Split();
            n = int.Parse(input[0]);
            k = int.Parse(input[1]);

            Console.Write("Введите стоимости товаров через пробел: ");
            string[] costsInput = Console.ReadLine().Split();
            costs = Array.ConvertAll(costsInput, int.Parse);
        }
        else if (mode == "2")
        {
            while (true)
            {
                Console.Write("\nВведите количество типов товаров (n) и количество товаров (k): ");
                string line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Ошибка: пустой ввод. Введите два числа через пробел.");
                    continue;
                }

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.WriteLine("Ошибка: нужно ввести ровно два числа.");
                    continue;
                }

                if (int.TryParse(parts[0], out int nVal) && int.TryParse(parts[1], out int kVal))
                {
                    if (nVal <= 0 || kVal <= 0)
                    {
                        Console.WriteLine("Ошибка: числа должны быть больше нуля.");
                        continue;
                    }

                    n = nVal;
                    k = kVal;
                    break;
                }
                else
                {
                    Console.WriteLine("Ошибка: введены нецелые числа или нечисловые символы.");
                }
            }

            costs = RandomGenerator.GenerateRandomCosts(n);
            Console.WriteLine("\nСгенерированные стоимости товаров: " + string.Join(" ", costs));
        }
        else if (mode == "3")
        {
            n = 3;
            k = 3;
            costs = new int[] { 1, 3, 4 };
            Console.WriteLine("\nДемонстрационный пример:");
            Console.WriteLine($"  n = {n}, k = {k}");
            Console.WriteLine($"  Стоимости товаров: [{string.Join(", ", costs)}]");
        }
        else
        {
            Console.WriteLine("Неверный режим.");
            return;
        }

        Console.WriteLine("\nРешение задачи...");
        var sw = Stopwatch.StartNew();
        List<int> possibleSums = ShopRobber.Solve(n, k, costs);
        sw.Stop();

        Console.WriteLine($"\nВремя решения: {sw.ElapsedMilliseconds} мс");
        Console.WriteLine($"Количество достижимых сумм: {possibleSums.Count}");

        if (possibleSums.Count <= 50)
        {
            Console.WriteLine("\nВозможные суммы стоимостей:");
            Console.WriteLine(string.Join(" ", possibleSums));
        }
        else
        {
            Console.WriteLine("\nВозможные суммы стоимостей (первые 50):");
            Console.WriteLine(string.Join(" ", possibleSums.Take(50)) + " ...");
            Console.WriteLine($"Последние 10: {string.Join(" ", possibleSums.TakeLast(10))}");
        }

        int minCost = costs.Min();
        int maxCost = costs.Max();
        Console.WriteLine($"\nОжидаемая минимальная сумма: {minCost * k}");
        Console.WriteLine($"Ожидаемая максимальная сумма: {maxCost * k}");
    }

    public static class RandomGenerator
    {
        private static readonly Random random = new Random();
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

    private static void CheckFFTCorrectness()
    {
        Console.WriteLine("\n=== Проверка корректности FFT/IFFT ===\n");

        int size = 1024;
        Random rand = new Random(42);

        Complex[] original = new Complex[size];

        for (int i = 0; i < size; i++)
        {
            original[i] = new Complex(rand.NextDouble(), rand.NextDouble());
        }

        Complex[] transformed = new Complex[size];
        Array.Copy(original, transformed, size);

        Console.WriteLine($"Размер массива: {size}");
        Console.WriteLine("Применяем прямое БПФ...");

        var sw = Stopwatch.StartNew();
        FFT.ComputeFFT(transformed);
        sw.Stop();
        Console.WriteLine($"  Время FFT: {sw.Elapsed.TotalMilliseconds:F3} мс");

        Console.WriteLine("Применяем обратное БПФ...");
        sw.Restart();
        FFT.ComputeIFFT(transformed);
        sw.Stop();
        Console.WriteLine($"  Время IFFT: {sw.Elapsed.TotalMilliseconds:F3} мс");

        double maxError = 0;

        for (int i = 0; i < size; i++)
        {
            double error = Complex.Abs(original[i] - transformed[i]);

            if (error > maxError)
            {
                maxError = error;
            }
        }

        Console.WriteLine($"\nМаксимальная ошибка восстановления: {maxError:E6}");

        if (maxError < 1e-9)
        {
            Console.WriteLine("✓ FFT/IFFT работают корректно.");
        }
        else
        {
            Console.WriteLine("✗ Обнаружена значительная ошибка.");
        }
    }
}