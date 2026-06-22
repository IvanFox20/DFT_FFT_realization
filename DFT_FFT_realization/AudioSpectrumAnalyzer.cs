using System;
using System.Numerics;
using ScottPlot;

namespace DFT_FFT_realization
{
    public class AudioSpectrumAnalyzer
    {
        public static void AnalyzeSignal()
        {
            int sampleRate = 44100;
            int sampleCount = 4096;

            double frequency1 = 440; 
            double frequency2 = 880;

            double[] signal = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                double t = (double)i / sampleRate;

                signal[i] =
                    Math.Sin(2 * Math.PI * frequency1 * t) +
                    0.5 * Math.Sin(2 * Math.PI * frequency2 * t);
            }

            double[] originalSignal = (double[])signal.Clone();

            // Временная шкала

            double[] time = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                time[i] = (double)i / sampleRate;
            }

            // Графки исходного сигнала

            var signalPlot = new ScottPlot.Plot();

            signalPlot.Add.Scatter(time, originalSignal);

            signalPlot.Title("Исходный аудиосигнал");

            signalPlot.XLabel("Время (сек)");
            signalPlot.YLabel("Амплитуда");

            signalPlot.Axes.SetLimits(0, 0.02);

            signalPlot.SavePng("signal.png", 1200, 600);

            Console.WriteLine("Сигнал сохранён в signal.png");

            ApplyHammingWindow(signal);

            // График после применения окна Хэмминга

            var hammingPlot = new ScottPlot.Plot();

            hammingPlot.Add.Scatter(time, signal);

            hammingPlot.Title("Сигнал после применения окна Хэмминга");

            hammingPlot.XLabel("Время (сек)");
            hammingPlot.YLabel("Амплитуда");

            hammingPlot.Axes.SetLimits(0, 0.02);

            hammingPlot.SavePng("signal_hamming.png", 1200, 600);

            Console.WriteLine("Сигнал после окна Хэмминга сохранён в signal_hamming.png");

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
                frequencies[i] =
                    (double)i * sampleRate / sampleCount;

                magnitudes[i] =
                    fftData[i].Magnitude;
            }

            // График спектра

            var spectrumPlot = new ScottPlot.Plot();

            spectrumPlot.Add.Scatter(frequencies, magnitudes);

            spectrumPlot.Title("Амплитудный спектр сигнала");

            spectrumPlot.XLabel("Частота (Гц)");
            spectrumPlot.YLabel("Амплитуда");

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