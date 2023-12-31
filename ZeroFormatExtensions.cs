using System;
using UnityEngine;

namespace Fury.Strings
{
    [Flags]
    public enum TimerFormatFlags : byte
    {
        Hours = 1 << 0
    }

    public static class ZeroFormatExtensions
    {
        public static unsafe void Append(this ZeroFormat format, int number, byte @base = 10)
        {
            if (@base < 2 || @base > 10 + 26)
            {
                throw new ArgumentOutOfRangeException(nameof(@base));
            }

            if (number == 0)
            {
                format.Append('0');
                return;
            }

            if (number  < 0)
            {
                format.Append('-');
                number = -number;
            }
            var numDigits = 0;
            for (var n = number; n > 0; n /= @base) numDigits++;
            var digits = stackalloc char[numDigits];

            var pos = numDigits - 1;
            while (number > 0)
            {
                var digit = number % @base;
                if (digit < 10)
                {
                    digits[pos--] = (char)('0' + digit);
                } else
                {
                    digits[pos--] = (char)('a' + digit - 10);
                }
                number = number / @base;
            }
            format.Append(digits, numDigits);
        }

        public enum AfterDecimalFormat
        {
            MaxNonZero,
            Fixed
        }

        public static unsafe void Append(this ZeroFormat format, float number, AfterDecimalFormat afterFormat, sbyte afterDecimalSize)
        {
            if (float.IsNaN(number))
            {
                format.Append("NaN");
                return;
            }
            if (float.IsPositiveInfinity(number))
            {
                format.Append("Inf+");
                return;
            }
            if (float.IsNegativeInfinity(number))
            {
                format.Append("Inf-");
                return;
            }

            var intNumber = (int)number;
            format.Append(intNumber);
            var fraction = Mathf.Abs(number - intNumber);

            if (afterFormat == AfterDecimalFormat.Fixed)
            {
                format.Append('.');

                for (var i = 0; i < afterDecimalSize; i++)
                {
                    fraction = fraction * 10f;
                    var digit = (int)(fraction);
                    format.Append((char)('0' + digit));
                    fraction -= digit;
                }
            } else
            {
                var fraction0 = fraction;

                var count = 0;
                for (var i = 0; i < afterDecimalSize; i++)
                {
                    fraction = fraction * 10f;
                    var digit = (int)(fraction);
                    fraction -= digit;
                    if (digit != 0)
                    {
                        count = i + 1;
                    }
                }

                if (count == 0)
                {
                    return;
                }
                format.Append('.');

                fraction = fraction0;
                for (var i = 0; i < count; i++)
                {
                    fraction = fraction * 10f;
                    var digit = (int)(fraction);
                    format.Append((char)('0' + digit));
                    fraction -= digit;
                }
            }
        }

        public static void AppendTimer(this ZeroFormat format, int seconds, TimerFormatFlags flags)
        {
            var ts = TimeSpan.FromSeconds(seconds);

            var hasHours = (flags & TimerFormatFlags.Hours) != 0;
            if (hasHours)
            {
                var hours = (int)ts.TotalHours;
                if (hours < 10)
                {
                    format.Append('0');
                }
                format.Append(hours);
                format.Append(":");
            }

            var min = hasHours ? ts.Minutes : (int)ts.TotalMinutes;
            if (min < 10)
            {
                format.Append('0');
            }
            format.Append(min);
            format.Append(':');

            var sec = ts.Seconds;
            if (sec < 10)
            {
                format.Append('0');
            }
            format.Append(sec);
        }
    }
}
