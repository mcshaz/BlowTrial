using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsForAge
{
    public static class Stats
    {
        public static double CumSnorm(Double Zscore)
        {
            double returnVal, ZAbs = Math.Abs(Zscore);
            if (ZAbs > 37)
            {
                return 0;
            }
            else
            {
                double Exponential = Math.Exp(-Math.Pow(ZAbs, 2) / 2);
                if (ZAbs < 7.07106781186547)
                {
                    double build = 3.52624965998911E-02 * ZAbs + 0.700383064443688;
                    build = build * ZAbs + 6.37396220353165;
                    build = build * ZAbs + 33.912866078383;
                    build = build * ZAbs + 112.079291497871;
                    build = build * ZAbs + 221.213596169931;
                    build = build * ZAbs + 220.206867912376;
                    returnVal = Exponential * build;
                    build = 8.83883476483184E-02 * ZAbs + 1.75566716318264;
                    build = build * ZAbs + 16.064177579207;
                    build = build * ZAbs + 86.7807322029461;
                    build = build * ZAbs + 296.564248779674;
                    build = build * ZAbs + 637.333633378831;
                    build = build * ZAbs + 793.826512519948;
                    build = build * ZAbs + 440.413735824752;
                    returnVal = returnVal / build;
                }
                else
                {
                    double build = ZAbs + 0.65;
                    build = ZAbs + 4 / build;
                    build = ZAbs + 3 / build;
                    build = ZAbs + 2 / build;
                    build = ZAbs + 1 / build;
                    returnVal = Exponential / build / 2.506628274631;
                }
            }
            return (Zscore < 0) ? returnVal : 1 - returnVal;
        }
        /// <summary>
        /// Returns the Z score for a value given statistical parameters
        /// </summary>
        /// <param name="xVal">measurement for evaluation</param>
        /// <param name="M">median</param>
        /// <param name="S">generalized coefficient of variation</param>
        /// <param name="L">power of box-cox transformation(optional)</param>
        /// <returns>Z</returns>
        public static double ZfromParams(double xVal, double M, double S, double L = 0)
        {
            if (L == 0)
            {
                return Math.Log(xVal / M) / S;
            }
            return (Math.Pow(xVal / M, L) - 1) / (L * S);
        }
        /// <summary>
        /// to obtain the value of a measurement for a particular Z score
        /// </summary>
        /// <param name="Z">Z score</param>
        /// <param name="M">median</param>
        /// <param name="S">generalized coefficient of variation</param>
        /// <param name="L">power of box-cox transformation(optional)</param>
        /// <returns>value of a measurement given statistical parameters </returns>
        public static double XfromParams(double Z, double M, double S, double L = 0)
        {
            if (L == 0)
            {
                return M * Math.Exp(S * Z);
            }
            else
            {
                return M * Math.Pow(1 + (L * S * Z), 1 / L);
            }
        }
        public static double InverseCumSnorm(double p)
        {
            // Coefficients in rational approximations
            double[] a = {-3.969683028665376e+01,  2.209460984245205e+02,
                                -2.759285104469687e+02,  1.383577518672690e+02,
                                -3.066479806614716e+01,  2.506628277459239e+00};

            double[] b = {-5.447609879822406e+01,  1.615858368580409e+02,
                                -1.556989798598866e+02,  6.680131188771972e+01,
                                -1.328068155288572e+01 };

            double[] c = {-7.784894002430293e-03, -3.223964580411365e-01,
                                -2.400758277161838e+00, -2.549732539343734e+00,
                                4.374664141464968e+00,  2.938163982698783e+00};

            double[] d =  {7.784695709041462e-03, 3.224671290700398e-01,
                                2.445134137142996e+00,  3.754408661907416e+00};

            // Define break-points.
            Double plow = 0.02425;
            Double phigh = 1 - plow;

            // Rational approximation for lower region:
            if (p < plow)
            {
                Double q = Math.Sqrt(-2 * Math.Log(p));
                return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }

            // Rational approximation for upper region:
            else if (phigh < p)
            {
                Double q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                        ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }
            else
            {
                // Rational approximation for central region:
                Double q = p - 0.5;
                Double r = q * q;
                return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
                                            (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
            }
        }
    }
}
