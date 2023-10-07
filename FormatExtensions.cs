using System;

namespace Fury.Strings
{
    public static class FormatExtensions
    {
        public static unsafe void Append(this Format format, int number, int @base = 10)
        {
            if (@base < 2 || @base > 10 + 26)
            {
                throw new ArgumentOutOfRangeException(nameof(@base));
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
    }
}
