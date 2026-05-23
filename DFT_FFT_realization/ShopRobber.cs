using System;
using System.Numerics;
using System.Collections.Generic;

public class ShopRobber
{
    // Метод для решения задачи "Вор в магазине"
    public static List<int> Solve(int n, int k, int[] costs)
    {
        // Определяем максимальную стоимость товара
        int maxCost = 0;
        foreach (var cost in costs)
        {
            if (cost > maxCost) maxCost = cost;
        }

        // Максимальная возможная сумма
        long maxSum = (long)maxCost * k;

        // Создаем полином P(x), где коэффициенты при x^a_i равны 1
        double[] polynomial = new double[maxSum + 1];
        foreach (var cost in costs)
        {
            polynomial[cost] = 1;
        }

        // Быстрое возведение полинома в степень k
        double[] result = PolynomialPower(polynomial, k);

        // Собираем все возможные суммы
        List<int> possibleSums = new List<int>();
        for (int i = 0; i <= maxSum; i++)
        {
            if (Math.Abs(result[i]) > 1e-6) // Проверяем, что коэффициент достаточно большой
            {
                possibleSums.Add(i);
            }
        }

        return possibleSums;
    }

    // Метод для быстрого возведения полинома в степень
    private static double[] PolynomialPower(double[] poly, int power)
    {
        int n = poly.Length;
        double[] result = new double[n];
        result[0] = 1; // Начинаем с единичного полинома

        while (power > 0)
        {
            if ((power & 1) == 1) // Если текущая степень нечётная
            {
                result = PolynomialMultiplication.MultiplyUsingFFT(result, poly);
            }
            poly = PolynomialMultiplication.MultiplyUsingFFT(poly, poly);
            power >>= 1; // Делим степень на 2
        }

        return result;
    }
}