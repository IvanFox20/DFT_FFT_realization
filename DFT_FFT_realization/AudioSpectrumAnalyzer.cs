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
            double frequency1 = 440;   // Ля первой октавы
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

            // Сохраняем исходный сигнал для визуализации
            double[] originalSignal = (double[])signal.Clone();

            // ---------- ВРЕМЕННАЯ ШКАЛА ----------

            double[] time = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                time[i] = (double)i / sampleRate;
            }

            // ---------- ГРАФИК ИСХОДНОГО СИГНАЛА ----------

            var signalPlot = new ScottPlot.Plot();

            signalPlot.Add.Scatter(time, originalSignal);

            signalPlot.Title("Исходный аудиосигнал");

            signalPlot.XLabel("Время (сек)");
            signalPlot.YLabel("Амплитуда");

            // Показываем только первые 0.02 секунды
            signalPlot.Axes.SetLimits(0, 0.02);

            signalPlot.SavePng("signal.png", 1200, 600);

            Console.WriteLine("Сигнал сохранён в signal.png");

            // ---------- ПРИМЕНЕНИЕ ОКНА ХЭММИНГА ----------

            ApplyHammingWindow(signal);

            // ---------- ПОДГОТОВКА ДАННЫХ ДЛЯ FFT ----------

            Complex[] fftData = new Complex[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                fftData[i] = new Complex(signal[i], 0);
            }

            // ---------- FFT ----------

            FFT.ComputeFFT(fftData);

            // ---------- НОРМИРОВКА ----------

            for (int i = 0; i < sampleCount; i++)
            {
                fftData[i] /= sampleCount;
            }

            // ---------- АМПЛИТУДНЫЙ СПЕКТР ----------

            int spectrumSize = sampleCount / 2;

            double[] frequencies = new double[spectrumSize];
            double[] magnitudes = new double[spectrumSize];

            for (int i = 0; i < spectrumSize; i++)
            {
                frequencies[i] =
                    (double)i * sampleRate / sampleCount;

                magnitudes[i] =
                    fftData[i].Magnitude;
            }

            // ---------- ГРАФИК СПЕКТРА ----------

            var spectrumPlot = new ScottPlot.Plot();

            spectrumPlot.Add.Scatter(frequencies, magnitudes);

            spectrumPlot.Title("Амплитудный спектр сигнала");

            spectrumPlot.XLabel("Частота (Гц)");
            spectrumPlot.YLabel("Амплитуда");

            // Отображаем диапазон до 1000 Гц
            spectrumPlot.Axes.SetLimits(0, 1000);

            spectrumPlot.SavePng("spectrum.png", 1200, 600);

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
                    0.46 *
                    Math.Cos(2 * Math.PI * i / (N - 1));

                signal[i] *= w;
            }
        }
    }
}