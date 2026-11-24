// Port of the fdlibm-derived implementations that V8 (and therefore the
// TypeScript reference implementation) uses for Math.sin, Math.cos,
// Math.atan and Math.atan2.
//
// The .NET runtime on Windows uses the UCRT implementations, which differ
// from V8 in the last bit for some arguments. Shape outlines are built from
// trigonometric points, so a one-ulp difference there can flip a
// mathematically tied feature-mapping distance and change the morph
// segmentation. These routines reproduce the fdlibm results bit-for-bit.
//
// Adapted from fdlibm (http://www.netlib.org/fdlibm):
// Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
// Developed at SunSoft, a Sun Microsystems, Inc. business.
// Permission to use, copy, modify, and distribute this software is freely
// granted, provided that this notice is preserved.

using System;

namespace Cccc.MaterialShape
{
    internal static class FreelyDistributableMathLibrary
    {
        private static int HighWord(double value)
        {
            return unchecked((int)(BitConverter.DoubleToInt64Bits(value) >> 32));
        }

        private static uint LowWord(double value)
        {
            return unchecked((uint)BitConverter.DoubleToInt64Bits(value));
        }

        private static double FromWords(int high, uint low)
        {
            long bits = ((long)(uint)high << 32) | low;
            return BitConverter.Int64BitsToDouble(bits);
        }

        private static double WithHighWord(double value, int high)
        {
            long bits = (BitConverter.DoubleToInt64Bits(value) & 0xFFFFFFFFL) | ((long)(uint)high << 32);
            return BitConverter.Int64BitsToDouble(bits);
        }

        private static double WithLowWord(double value, uint low)
        {
            long bits = (BitConverter.DoubleToInt64Bits(value) & unchecked((long)0xFFFFFFFF00000000UL)) | low;
            return BitConverter.Int64BitsToDouble(bits);
        }

        // ---------------------------------------------------------------
        // Argument reduction: x = (n * pi/2) + y[0] + y[1]
        // ---------------------------------------------------------------

        private static readonly int[] TwoOverPi =
        {
            0xA2F983, 0x6E4E44, 0x1529FC, 0x2757D1, 0xF534DD, 0xC0DB62, 0x95993C,
            0x439041, 0xFE5163, 0xABDEBB, 0xC561B7, 0x246E3A, 0x424DD2, 0xE00649,
            0x2EEA09, 0xD1921C, 0xFE1DEB, 0x1CB129, 0xA73EE8, 0x8235F5, 0x2EBB44,
            0x84E99C, 0x7026B4, 0x5F7E41, 0x3991D6, 0x398353, 0x39F49C, 0x845F8B,
            0xBDF928, 0x3B1FF8, 0x97FFDE, 0x05980F, 0xEF2F11, 0x8B5A0A, 0x6D1F6D,
            0x367ECF, 0x27CB09, 0xB74F46, 0x3F669E, 0x5FEA2D, 0x7527BA, 0xC7EBE5,
            0xF17B3D, 0x0739F7, 0x8A5292, 0xEA6BFB, 0x5FB11F, 0x8D5D08, 0x560330,
            0x46FC7B, 0x6BABF0, 0xCFBC20, 0x9AF436, 0x1DA9E3, 0x91615E, 0xE61B08,
            0x659985, 0x5F14A0, 0x68408D, 0xFFD880, 0x4D7327, 0x310606, 0x1556CA,
            0x73A8C9, 0x60E27B, 0xC08C6B,
        };

        private static readonly int[] Npio2Hw =
        {
            0x3FF921FB, 0x400921FB, 0x4012D97C, 0x401921FB, 0x401F6A7A, 0x4022D97C,
            0x4025FDBB, 0x402921FB, 0x402C463A, 0x402F6A7A, 0x4031475C, 0x4032D97C,
            0x40346B9C, 0x4035FDBB, 0x40378FDB, 0x403921FB, 0x403AB41B, 0x403C463A,
            0x403DD85A, 0x403F6A7A, 0x40407E4C, 0x4041475C, 0x4042106C, 0x4042D97C,
            0x4043A28C, 0x40446B9C, 0x404534AC, 0x4045FDBB, 0x4046C6CB, 0x40478FDB,
            0x404858EB, 0x404921FB,
        };

        private static readonly double[] Pio2 =
        {
            1.57079625129699707031e+00,
            7.54978941586159635335e-08,
            5.39030252995776476554e-15,
            3.28200341580791294123e-22,
            1.27065575308067607349e-29,
            1.22933308981111328932e-36,
            2.73370053816464559624e-44,
            2.16741683877804819444e-51,
        };

        private static int RemPio2(double x, out double y0, out double y1)
        {
            const double half = 5.00000000000000000000e-01;
            const double two24 = 1.67772160000000000000e+07;
            const double invpio2 = 6.36619772367581382433e-01;
            const double pio2_1 = 1.57079632673412561417e+00;
            const double pio2_1t = 6.07710050650619224932e-11;
            const double pio2_2 = 6.07710050630396597660e-11;
            const double pio2_2t = 2.02226624879595063154e-21;
            const double pio2_3 = 2.02226624871116645580e-21;
            const double pio2_3t = 8.47842766036889956997e-32;

            y0 = 0;
            y1 = 0;
            int hx = HighWord(x);
            int ix = hx & 0x7FFFFFFF;
            uint low = LowWord(x);

            if (ix <= 0x3FE921FB)
            {
                y0 = x;
                y1 = 0;
                return 0;
            }

            if (ix < 0x4002D97C)
            {
                if (hx > 0)
                {
                    double z = x - pio2_1;
                    if (ix != 0x3FF921FB)
                    {
                        y0 = z - pio2_1t;
                        y1 = (z - y0) - pio2_1t;
                    }
                    else
                    {
                        z -= pio2_2;
                        y0 = z - pio2_2t;
                        y1 = (z - y0) - pio2_2t;
                    }
                    return 1;
                }
                else
                {
                    double z = x + pio2_1;
                    if (ix != 0x3FF921FB)
                    {
                        y0 = z + pio2_1t;
                        y1 = (z - y0) + pio2_1t;
                    }
                    else
                    {
                        z += pio2_2;
                        y0 = z + pio2_2t;
                        y1 = (z - y0) + pio2_2t;
                    }
                    return -1;
                }
            }

            if (ix <= 0x413921FB)
            {
                double t = Math.Abs(x);
                int n = (int)(t * invpio2 + half);
                double fn = n;
                double r = t - fn * pio2_1;
                double w = fn * pio2_1t;
                if (n < 32 && ix != Npio2Hw[n - 1])
                {
                    y0 = r - w;
                }
                else
                {
                    int j = ix >> 20;
                    y0 = r - w;
                    int high = HighWord(y0);
                    int i = j - ((high >> 20) & 0x7FF);
                    if (i > 16)
                    {
                        t = r;
                        w = fn * pio2_2;
                        r = t - w;
                        w = fn * pio2_2t - ((t - r) - w);
                        y0 = r - w;
                        high = HighWord(y0);
                        i = j - ((high >> 20) & 0x7FF);
                        if (i > 49)
                        {
                            t = r;
                            w = fn * pio2_3;
                            r = t - w;
                            w = fn * pio2_3t - ((t - r) - w);
                            y0 = r - w;
                        }
                    }
                }
                y1 = (r - y0) - w;
                if (hx < 0)
                {
                    y0 = -y0;
                    y1 = -y1;
                    return -n;
                }
                return n;
            }

            if (ix >= 0x7FF00000)
            {
                y0 = x - x;
                y1 = x - x;
                return 0;
            }

            double zz = WithLowWord(0, low);
            int e0 = (ix >> 20) - 1046;
            zz = WithHighWord(zz, ix - (int)((uint)e0 << 20));
            double[] tx = new double[3];
            for (int i = 0; i < 2; i++)
            {
                tx[i] = (int)zz;
                zz = (zz - tx[i]) * two24;
            }
            tx[2] = zz;
            int nx = 3;
            while (tx[nx - 1] == 0) nx--;
            double[] yy = new double[3];
            int result = KernelRemPio2(tx, yy, e0, nx, 2, TwoOverPi);
            y0 = yy[0];
            y1 = yy[1];
            if (hx < 0)
            {
                y0 = -y0;
                y1 = -y1;
                return -result;
            }
            return result;
        }

        private static int KernelRemPio2(double[] x, double[] y, int e0, int nx, int prec, int[] ipio2)
        {
            int[] initJk = { 2, 3, 4, 6 };
            const double two24 = 1.67772160000000000000e+07;
            const double twon24 = 5.96046447753906250000e-08;

            int jk = initJk[prec];
            int jp = jk;
            int jx = nx - 1;
            int jv = (e0 - 3) / 24;
            if (jv < 0) jv = 0;
            int q0 = e0 - 24 * (jv + 1);

            double[] f = new double[20];
            double[] fq = new double[20];
            double[] q = new double[20];
            int[] iq = new int[20];

            int j = jv - jx;
            int m = jx + jk;
            for (int i = 0; i <= m; i++, j++)
            {
                f[i] = j < 0 ? 0.0 : ipio2[j];
            }

            for (int i = 0; i <= jk; i++)
            {
                double fw = 0.0;
                for (j = 0; j <= jx; j++) fw += x[j] * f[jx + i - j];
                q[i] = fw;
            }

            int jz = jk;
            int n;
            int ih;
            double z;
            int carry;

            recompute:
            int iLoop = 0;
            int jLoop = jz;
            z = q[jz];
            while (jLoop > 0)
            {
                double fw = (int)(twon24 * z);
                iq[iLoop] = (int)(z - two24 * fw);
                z = q[jLoop - 1] + fw;
                iLoop++;
                jLoop--;
            }

            z = Math.ScaleB(z, q0);
            z -= 8.0 * Math.Floor(z * 0.125);
            n = (int)z;
            z -= n;
            ih = 0;
            if (q0 > 0)
            {
                int i = iq[jz - 1] >> (24 - q0);
                n += i;
                iq[jz - 1] -= i << (24 - q0);
                ih = iq[jz - 1] >> (23 - q0);
            }
            else if (q0 == 0)
            {
                ih = iq[jz - 1] >> 23;
            }
            else if (z >= 0.5)
            {
                ih = 2;
            }

            if (ih > 0)
            {
                n += 1;
                carry = 0;
                for (int i = 0; i < jz; i++)
                {
                    int value = iq[i];
                    if (carry == 0)
                    {
                        if (value != 0)
                        {
                            carry = 1;
                            iq[i] = 0x1000000 - value;
                        }
                    }
                    else
                    {
                        iq[i] = 0xFFFFFF - value;
                    }
                }
                if (q0 > 0)
                {
                    switch (q0)
                    {
                        case 1:
                            iq[jz - 1] &= 0x7FFFFF;
                            break;
                        case 2:
                            iq[jz - 1] &= 0x3FFFFF;
                            break;
                    }
                }
                if (ih == 2)
                {
                    z = 1.0 - z;
                    if (carry != 0) z -= Math.ScaleB(1.0, q0);
                }
            }

            if (z == 0)
            {
                int any = 0;
                for (int i = jz - 1; i >= jk; i--) any |= iq[i];
                if (any == 0)
                {
                    int k = 1;
                    while (jk >= k && iq[jk - k] == 0) k++;
                    for (int i = jz + 1; i <= jz + k; i++)
                    {
                        f[jx + i] = ipio2[jv + i];
                        double fw = 0.0;
                        for (j = 0; j <= jx; j++) fw += x[j] * f[jx + i - j];
                        q[i] = fw;
                    }
                    jz += k;
                    goto recompute;
                }
            }

            if (z == 0.0)
            {
                jz -= 1;
                q0 -= 24;
                while (iq[jz] == 0)
                {
                    jz--;
                    q0 -= 24;
                }
            }
            else
            {
                z = Math.ScaleB(z, -q0);
                if (z >= two24)
                {
                    double fw = (int)(twon24 * z);
                    iq[jz] = (int)(z - two24 * fw);
                    jz += 1;
                    q0 += 24;
                    iq[jz] = (int)fw;
                }
                else
                {
                    iq[jz] = (int)z;
                }
            }

            double scale = Math.ScaleB(1.0, q0);
            for (int i = jz; i >= 0; i--)
            {
                q[i] = scale * iq[i];
                scale *= twon24;
            }

            for (int i = jz; i >= 0; i--)
            {
                double fw = 0.0;
                for (int k = 0; k <= jp && k <= jz - i; k++) fw += Pio2[k] * q[i + k];
                fq[jz - i] = fw;
            }

            switch (prec)
            {
                case 0:
                {
                    double fw = 0.0;
                    for (int i = jz; i >= 0; i--) fw += fq[i];
                    y[0] = ih == 0 ? fw : -fw;
                    break;
                }
                case 1:
                case 2:
                {
                    double fw = 0.0;
                    for (int i = jz; i >= 0; i--) fw += fq[i];
                    y[0] = ih == 0 ? fw : -fw;
                    fw = fq[0] - fw;
                    for (int i = 1; i <= jz; i++) fw += fq[i];
                    y[1] = ih == 0 ? fw : -fw;
                    break;
                }
                default:
                {
                    for (int i = jz; i > 0; i--)
                    {
                        double fw = fq[i - 1] + fq[i];
                        fq[i] += fq[i - 1] - fw;
                        fq[i - 1] = fw;
                    }
                    for (int i = jz; i > 1; i--)
                    {
                        double fw = fq[i - 1] + fq[i];
                        fq[i] += fq[i - 1] - fw;
                        fq[i - 1] = fw;
                    }
                    double fw2 = 0.0;
                    for (int i = jz; i >= 2; i--) fw2 += fq[i];
                    if (ih == 0)
                    {
                        y[0] = fq[0];
                        y[1] = fq[1];
                        y[2] = fw2;
                    }
                    else
                    {
                        y[0] = -fq[0];
                        y[1] = -fq[1];
                        y[2] = -fw2;
                    }
                    break;
                }
            }
            return n & 7;
        }

        // ---------------------------------------------------------------
        // Kernels on [-pi/4, pi/4]
        // ---------------------------------------------------------------

        private static double KernelSin(double x, double y, int iy)
        {
            const double half = 5.00000000000000000000e-01;
            const double s1 = -1.66666666666666324348e-01;
            const double s2 = 8.33333333332248946124e-03;
            const double s3 = -1.98412698298579493134e-04;
            const double s4 = 2.75573137070700676789e-06;
            const double s5 = -2.50507602534068634195e-08;
            const double s6 = 1.58969099521155010221e-10;

            int ix = HighWord(x) & 0x7FFFFFFF;
            if (ix < 0x3E400000 && (int)x == 0) return x;
            double z = x * x;
            double v = z * x;
            double r = s2 + z * (s3 + z * (s4 + z * (s5 + z * s6)));
            if (iy == 0) return x + v * (s1 + z * r);
            return x - ((z * (half * y - v * r) - y) - v * s1);
        }

        private static double KernelCos(double x, double y)
        {
            const double one = 1.00000000000000000000e+00;
            const double c1 = 4.16666666666666019037e-02;
            const double c2 = -1.38888888888741095749e-03;
            const double c3 = 2.48015872894767294178e-05;
            const double c4 = -2.75573143513906633035e-07;
            const double c5 = 2.08757232129817482790e-09;
            const double c6 = -1.13596475577881948265e-11;

            int ix = HighWord(x) & 0x7FFFFFFF;
            if (ix < 0x3E400000 && (int)x == 0) return one;
            double z = x * x;
            double r = z * (c1 + z * (c2 + z * (c3 + z * (c4 + z * (c5 + z * c6)))));
            if (ix < 0x3FD33333) return one - (0.5 * z - (z * r - x * y));
            double qx;
            if (ix > 0x3FE90000)
            {
                qx = 0.28125;
            }
            else
            {
                qx = FromWords(ix - 0x00200000, 0);
            }
            double iz = 0.5 * z - qx;
            double a = one - qx;
            return a - (iz - (z * r - x * y));
        }

        // ---------------------------------------------------------------
        // Public entry points
        // ---------------------------------------------------------------

        internal static double Sin(double x)
        {
            int ix = HighWord(x) & 0x7FFFFFFF;
            if (ix <= 0x3FE921FB) return KernelSin(x, 0.0, 0);
            if (ix >= 0x7FF00000) return x - x;
            int n = RemPio2(x, out double y0, out double y1);
            switch (n & 3)
            {
                case 0: return KernelSin(y0, y1, 1);
                case 1: return KernelCos(y0, y1);
                case 2: return -KernelSin(y0, y1, 1);
                default: return -KernelCos(y0, y1);
            }
        }

        internal static double Cos(double x)
        {
            int ix = HighWord(x) & 0x7FFFFFFF;
            if (ix <= 0x3FE921FB) return KernelCos(x, 0.0);
            if (ix >= 0x7FF00000) return x - x;
            int n = RemPio2(x, out double y0, out double y1);
            switch (n & 3)
            {
                case 0: return KernelCos(y0, y1);
                case 1: return -KernelSin(y0, y1, 1);
                case 2: return -KernelCos(y0, y1);
                default: return KernelSin(y0, y1, 1);
            }
        }

        internal static double Atan(double x)
        {
            double[] atanhi =
            {
                4.63647609000806093515e-01,
                7.85398163397448278999e-01,
                9.82793723247329054082e-01,
                1.57079632679489655800e+00,
            };
            double[] atanlo =
            {
                2.26987774529616870924e-17,
                3.06161699786838301793e-17,
                1.39033110312309984516e-17,
                6.12323399573676603587e-17,
            };
            double[] aT =
            {
                3.33333333333329318027e-01,
                -1.99999999998764832476e-01,
                1.42857142725034663711e-01,
                -1.11111104054623557880e-01,
                9.09088713343650656196e-02,
                -7.69187620504482999495e-02,
                6.66107313738753120669e-02,
                -5.83357013379057348645e-02,
                4.97687799461593236017e-02,
                -3.65315727442169155270e-02,
                1.62858201153657823623e-02,
            };

            const double one = 1.0;
            const double huge = 1.0e300;

            int hx = HighWord(x);
            int ix = hx & 0x7FFFFFFF;
            if (ix >= 0x44100000)
            {
                if (ix > 0x7FF00000 || (ix == 0x7FF00000 && LowWord(x) != 0)) return x + x;
                if (hx > 0) return atanhi[3] + atanlo[3];
                return -atanhi[3] - atanlo[3];
            }

            int id;
            if (ix < 0x3FDC0000)
            {
                if (ix < 0x3E400000 && huge + x > one) return x;
                id = -1;
            }
            else
            {
                x = Math.Abs(x);
                if (ix < 0x3FF30000)
                {
                    if (ix < 0x3FE60000)
                    {
                        id = 0;
                        x = (2.0 * x - one) / (2.0 + x);
                    }
                    else
                    {
                        id = 1;
                        x = (x - one) / (x + one);
                    }
                }
                else
                {
                    if (ix < 0x40038000)
                    {
                        id = 2;
                        x = (x - 1.5) / (one + 1.5 * x);
                    }
                    else
                    {
                        id = 3;
                        x = -1.0 / x;
                    }
                }
            }

            double z = x * x;
            double w = z * z;
            double s1 = z * (aT[0] + w * (aT[2] + w * (aT[4] + w * (aT[6] + w * (aT[8] + w * aT[10])))));
            double s2 = w * (aT[1] + w * (aT[3] + w * (aT[5] + w * (aT[7] + w * aT[9]))));
            if (id < 0) return x - x * (s1 + s2);
            z = atanhi[id] - ((x * (s1 + s2) - atanlo[id]) - x);
            return hx < 0 ? -z : z;
        }

        internal static double Atan2(double y, double x)
        {
            const double tiny = 1.0e-300;
            const double zero = 0.0;
            const double piOver4 = 7.8539816339744827900E-01;
            const double piOver2 = 1.5707963267948965580E+00;
            const double pi = 3.1415926535897931160E+00;
            const double piLo = 1.2246467991473531772E-16;

            int hx = HighWord(x);
            int lx = unchecked((int)LowWord(x));
            int ix = hx & 0x7FFFFFFF;
            int hy = HighWord(y);
            int ly = unchecked((int)LowWord(y));
            int iy = hy & 0x7FFFFFFF;

            if (double.IsNaN(x) || double.IsNaN(y)) return x + y;
            if ((hx - 0x3FF00000 | lx) == 0) return Atan(y);

            int m = ((hy >> 31) & 1) | ((hx >> 30) & 2);

            if ((iy | ly) == 0)
            {
                switch (m)
                {
                    case 0:
                    case 1:
                        return y;
                    case 2:
                        return pi + tiny;
                    default:
                        return -pi - tiny;
                }
            }

            if ((ix | lx) == 0) return hy < 0 ? -piOver2 - tiny : piOver2 + tiny;

            if (ix == 0x7FF00000)
            {
                if (iy == 0x7FF00000)
                {
                    switch (m)
                    {
                        case 0: return piOver4 + tiny;
                        case 1: return -piOver4 - tiny;
                        case 2: return 3.0 * piOver4 + tiny;
                        default: return -3.0 * piOver4 - tiny;
                    }
                }
                switch (m)
                {
                    case 0: return zero;
                    case 1: return -zero;
                    case 2: return pi + tiny;
                    default: return -pi - tiny;
                }
            }

            if (iy == 0x7FF00000) return hy < 0 ? -piOver2 - tiny : piOver2 + tiny;

            int k = (iy - ix) >> 20;
            double z;
            if (k > 60)
            {
                z = piOver2 + 0.5 * piLo;
                m &= 1;
            }
            else if (hx < 0 && k < -60)
            {
                z = 0.0;
            }
            else
            {
                z = Atan(Math.Abs(y / x));
            }

            switch (m)
            {
                case 0: return z;
                case 1: return -z;
                case 2: return pi - (z - piLo);
                default: return (z - piLo) - pi;
            }
        }
    }
}
