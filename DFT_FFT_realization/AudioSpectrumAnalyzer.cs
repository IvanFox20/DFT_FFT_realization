using System;
using System.Numerics;
using ScottPlot;

namespace DFT_FFT_realization
{
    public class AudioSpectrumAnalyzer
    {
        public static void AnalyzeSignal()
        {
            // Параметры сигнала
            int sampleRate = 44100;
            int sampleCount = 4096;

            // Частоты сигнала
            double frequency1 = 440;   // Ля
            double frequency2 = 880;   // Вторая гармоника

            // Генерация сигнала
            double[] signal = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                double t = (double)i / sampleRate;

                signal[i] =
                    Math.Sin(2 * Math.PI * frequency1 * t) +
                    0.5 * Math.Sin(2 * Math.PI * frequency2 * t);
            }

            // Применение окна Хэмминга
            ApplyHammingWindow(signal);

            // Подготовка данных для FFT
            Complex[] fftData = new Complex[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                fftData[i] = new Complex(signal[i], 0);
            }

            // FFT
            FFT.ComputeFFT(fftData);

            // Нормировка
            for (int i = 0; i < sampleCount; i++)
            {
                fftData[i] /= sampleCount;
            }

            // Амплитудный спектр
            int spectrumSize = sampleCount / 2;

            double[] frequencies = new double[spectrumSize];
            double[] magnitudes = new double[spectrumSize];

            for (int i = 0; i < spectrumSize; i++)
            {
                frequencies[i] = (double)i * sampleRate / sampleCount;
                magnitudes[i] = fftData[i].Magnitude;
            }

            // Временная шкала
            double[] time = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                time[i] = (double)i / sampleRate;
            }

            // Построение графика сигнала
            var signalPlot = new ScottPlot.Plot();

            signalPlot.Add.Scatter(time, signal);

            signalPlot.Title("Исходный аудиосигнал");

            signalPlot.XLabel("Время (сек)");
            signalPlot.YLabel("Амплитуда");

            // Показываем только первую 0.1 секунды
            signalPlot.Axes.SetLimits(0, 0.1);

            signalPlot.SavePng("signal.png", 1200, 600);

            Console.WriteLine("Сигнал сохранён в signal.png");

            // Построение графика
            var plt = new ScottPlot.Plot();

            plt.Add.Scatter(frequencies, magnitudes);

            plt.Title("Амплитудный спектр сигнала");

            plt.XLabel("Частота (Гц)");
            plt.YLabel("Амплитуда");

            plt.Axes.SetLimits(0, 1000);

            plt.SavePng("spectrum.png", 1200, 1000);

            Console.WriteLine("Спектр сохранён в spectrum.png");
        }

        // Оконная функция Хэмминга
        private static void ApplyHammingWindow(double[] signal)
        {
            int N = signal.Length;

            for (int i = 0; i < N; i++)
            {
                double w =
                    0.54 -
                    0.46 * Math.Cos(2 * Math.PI * i / (N - 1));

                signal[i] *= w;
            }
        }
    }
}
