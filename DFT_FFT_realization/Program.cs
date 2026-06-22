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