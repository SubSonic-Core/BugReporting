using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace SubSonic
{
    /// <summary>
    /// Local unique identifier
    /// </summary>
    /// <remarks>
    /// 16 character hex digit lowercase identifier
    /// </remarks>
    [JsonConverter(typeof(Serialization.LuidJsonConverter))]
    public readonly struct Luid
    {
        internal const string _valid_chars = "0123456789abcdf";    // hex digit values

        private static readonly int _max = 16;
        private readonly char[] _value;
        private readonly byte[] _salt;

        public static Luid Create()
        {
            return Create("", true);
        }

        public static Luid Create(Guid? guid, bool randomize = false)
        {
            if (guid.HasValue)
            {
                return Create(guid.GetValueOrDefault().ToString("N")[_max..], randomize);
            }
            return Create("", randomize);
        }

        public static Luid Create(string? value, bool randomize = false)
        {
            var luid = new char[_max];

            if (string.IsNullOrWhiteSpace(value))
            {
                value = "";
            }

            value = value.PadLeft(_max, _valid_chars[0]);

            for (int i = 0; i < _max; i++)
            {
#if NET8_0_OR_GREATER
                if (char.IsAsciiHexDigitLower(value[i]))
#else
                if (value[i].IsHexDigitLower())
#endif
                {
                    luid[i] = value[i];
                }
                else
                {
                    throw new ApplicationException($"'{value}' is not valid input, only lower case hex digit characters accepted!");
                }
            }

            return randomize ? new Luid(luid, randomize) : new Luid(luid);
        }

        public static bool TryParse(object value, out Luid? luid)
        {
            luid = null;

            if (value is string @string &&
                @string.Length <= _max)
            {
                luid = Create(@string);
            }
            if (value is Guid guid)
            {
                luid = Create(guid);
            }

            return luid.HasValue;   
        }

        public Luid(char[] value)
        {
            _salt = null!;
            _value = value;
        }

        public Luid(ReadOnlySpan<byte> value)
            : this(value.ToHexLowerString().ToCharArray())
        {
        }

        public Luid(char[] value, bool randomize = false)
        {
            if (randomize)
            {
#if NET6_0_OR_GREATER
                _salt = RandomNumberGenerator.GetBytes(_max);

                int count = 0;

                for (int i = 0; i < (_max / 2); ++i)
                {
                    char[] hex = new char[2];

                    do
                    {
                        for (int j = 0; j < hex.Length; ++j)
                        {
                            count = (count + _salt[(i * 2) + j]) % (_valid_chars.Length - 1);

                            hex[j] = _valid_chars[count];
                        }
                    }
                    while ((hex[0] | hex[1]) == 0xFF);

                    value[i * 2] = hex[0];
                    value[i * 2 + 1] = hex[1];
                }
#else
                throw new NotSupportedException();
#endif
            }
            else
            {
                _salt = Array.Empty<byte>();
            }

            _value = value;
        }

        public void CopyTo(Span<byte> destination)
        {
            SetSpanFromHexChars(_value.AsSpan(), destination);
        }

        static void SetSpanFromHexChars(ReadOnlySpan<char> value, Span<byte> destination)
        {
            value.ToSpanFromHexChars(destination);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Luid luid)
            {
                bool equality = true;

                for(int i = 0;i < _max; i++)
                {
                    equality &= _value[i] == luid._value[i];
                }

                return equality;
            }
            if (obj is string @string)
            {
                return ToString().Equals(@string);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public bool IsEmpty()
        {
            bool result = true;

            for(int  i = 0; i < _max; ++i)
            {
                result &= _value[i] == _valid_chars[0];
            }

            return result;
        }

        public override string ToString()
        {
            return new string(_value, 0, _max);
        }

        public static implicit operator Luid(string? value) => Create(value);
        public static implicit operator string(Luid luid) => luid.ToString();
    }
}
