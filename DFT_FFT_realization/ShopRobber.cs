using System;
using System.Collections.Generic;

public class ShopRobber
{
    public static List<int> Solve(int n, int k, int[] costs)
    {
        int maxCost = 0;
        foreach (var cost in costs)
        {
            if (cost > maxCost) maxCost = cost;
        }

        long maxSum = (long)maxCost * k;

        // Используем long для NTT
        long[] polynomial = new long[maxSum + 1];
        foreach (var cost in costs)
        {
            polynomial[cost] = 1;
        }

        // Быстрое возведение в степень через NTT
        long[] result = PolynomialPowerNTT(polynomial, k, (int)maxSum);

        List<int> possibleSums = new List<int>();
        for (int i = 0; i <= maxSum; i++)
        {
            // Коэффициент > 0 означает достижимость
            if (result[i] > 0)
            {
                possibleSums.Add(i);
            }
        }

        return possibleSums;
    }

    private static long[] PolynomialPowerNTT(long[] poly, int power, int maxDegree)
    {
        long[] result = new long[poly.Length];
        result[0] = 1;

        while (power > 0)
        {
            if ((power & 1) == 1)
            {
                result = NTT.Multiply(result, poly);
                result = TrimPolynomial(result, maxDegree);
            }
            poly = NTT.Multiply(poly, poly);
            poly = TrimPolynomial(poly, maxDegree);
            power >>= 1;
        }

        return result;
    }

    private static long[] TrimPolynomial(long[] poly, int maxDegree)
    {
        if (poly.Length <= maxDegree + 1)
            return poly;

        long[] trimmed = new long[maxDegree + 1];
        Array.Copy(poly, trimmed, maxDegree + 1);
        return trimmed;
    }
}