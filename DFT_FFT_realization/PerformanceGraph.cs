using System;
using System.Diagnostics;
using ScottPlot;

namespace DFT_FFT_realization
{
    public static class PerformanceGraph
    {
        public static void BuildPerformanceGraph()
        {
            int[] sizes =
            {
                2,
                4,
                8,
                16,
                32,
                64,
                128,
                256,
                512,
                1024,
                2048,
                4096,
                8192,
                16384,
                32768,
                65536
            };

            double[] naiveTimes = new double[sizes.Length];
            double[] fftTimes = new double[sizes.Length];

            Random random = new Random();

            for (int i = 0; i < sizes.Length; i++)
            {
                int n = sizes[i];

                double[] a = new double[n];
                double[] b = new double[n];

                for (int j = 0; j < n; j++)
                {
                    a[j] = random.NextDouble();
                    b[j] = random.NextDouble();
                }

                // Наивное умножение
                Stopwatch sw = Stopwatch.StartNew();

                PolynomialMultiplication
                    .MultiplyNaive(a, b);

                sw.Stop();

                naiveTimes[i] = sw.Elapsed.TotalMilliseconds;

                // FFT умножение
                sw.Restart();

                PolynomialMultiplication
                    .MultiplyUsingFFT(a, b);

                sw.Stop();

                fftTimes[i] = sw.Elapsed.TotalMilliseconds;

                Console.WriteLine(
                    $"N = {n} | Naive = {naiveTimes[i]:F3} ms | FFT = {fftTimes[i]:F3} ms");
            }

            // Построение графика
            var plt = new ScottPlot.Plot();

            plt.Add.Scatter(
                sizes,
                naiveTimes);

            plt.Add.Scatter(
                sizes,
                fftTimes);

            plt.Title("Сравнение производительности алгоритмов");
            plt.XLabel("Размер входных данных N");
            plt.YLabel("Время выполнения (мс)");

            plt.Legend.IsVisible = true;

            plt.SavePng(
                "performance.png",
                1200,
                800);

            Console.WriteLine(
                "График сохранён в performance.png");
        }
    }
}