namespace Fast.Iaas.Extension;

/// <summary>
/// 字符串扩展
/// </summary>
public static partial class Extensions
{
    private const byte Byte0 = 0;

    private const byte Byte1 = 1;

    private const ushort UShort0 = 0;

    private const ushort UShort1 = 1;

    private static byte ClearLeast(this byte value)
    {
        return (byte) (value & (value - 1));
    }

    private static ushort ClearLeast(this ushort value)
    {
        return (ushort) (value & (value - 1));
    }

    private static uint ClearLeast(this uint value)
    {
        return value & (value - 1);
    }

    private static ulong ClearLeast(this ulong value)
    {
        return value & (value - 1);
    }

    public static int Count(this byte value)
    {
        var num = 0;
        while (value != 0)
        {
            num++;
            value = (byte) (value & (value - 1));
        }

        return num;
    }

    public static int Count(this ushort value)
    {
        var num = 0;
        while (value != 0)
        {
            num++;
            value = (ushort) (value & (value - 1));
        }

        return num;
    }

    public static int Count(this uint value)
    {
        var num = 0;
        while (value != 0)
        {
            num++;
            value &= value - 1;
        }

        return num;
    }

    public static int Count(this ulong value)
    {
        var num = 0;
        while (value != 0L)
        {
            num++;
            value &= value - 1;
        }

        return num;
    }

    public static byte[] GetOnes(this byte value)
    {
        var num = 0;
        var array = new byte[8];
        while (value != 0)
        {
            var b = value;
            value = (byte) (value & (value - 1));
            array[num++] = (byte) (b - value);
        }

        var array2 = new byte[num];
        for (var i = 0; i < num; i++)
        {
            array2[i] = array[i];
        }

        return array2;
    }

    public static ushort[] GetOnes(this ushort value)
    {
        var num = 0;
        var array = new ushort[16];
        while (value != 0)
        {
            var num2 = value;
            value = (ushort) (value & (value - 1));
            array[num++] = (ushort) (num2 - value);
        }

        var array2 = new ushort[num];
        for (var i = 0; i < num; i++)
        {
            array2[i] = array[i];
        }

        return array2;
    }

    public static uint[] GetOnes(this uint value)
    {
        var num = 0;
        var array = new uint[32];
        while (value != 0)
        {
            var num2 = value;
            value &= value - 1;
            array[num++] = num2 - value;
        }

        var array2 = new uint[num];
        for (var i = 0; i < num; i++)
        {
            array2[i] = array[i];
        }

        return array2;
    }

    public static ulong[] GetOnes(this ulong value)
    {
        var num = 0;
        var array = new ulong[64];
        while (value != 0L)
        {
            var num2 = value;
            value &= value - 1;
            array[num++] = num2 - value;
        }

        var array2 = new ulong[num];
        for (var i = 0; i < num; i++)
        {
            array2[i] = array[i];
        }

        return array2;
    }

    public static bool ExactlyOne(this byte value)
    {
        if (value != 0)
        {
            return (value & (value - 1)) == 0;
        }

        return false;
    }

    public static bool ExactlyOne(this ushort value)
    {
        if (value != 0)
        {
            return (value & (value - 1)) == 0;
        }

        return false;
    }

    public static bool ExactlyOne(this uint value)
    {
        if (value != 0)
        {
            return (value & (value - 1)) == 0;
        }

        return false;
    }

    public static bool ExactlyOne(this ulong value)
    {
        if (value != 0L)
        {
            return (value & (value - 1)) == 0;
        }

        return false;
    }

    public static bool MoreThanOne(this byte value)
    {
        return (value & (value - 1)) != 0;
    }

    public static bool MoreThanOne(this ushort value)
    {
        return (value & (value - 1)) != 0;
    }

    public static bool MoreThanOne(this uint value)
    {
        return (value & (value - 1)) != 0;
    }

    public static bool MoreThanOne(this ulong value)
    {
        return (value & (value - 1)) != 0;
    }

    public static sbyte[] GetOnes(this sbyte value)
    {
        var ones = GetOnes((byte) value);
        var num = ones.Length;
        var array = new sbyte[num];
        for (var i = 0; i < num; i++)
        {
            array[i] = (sbyte) ones[i];
        }

        return array;
    }

    public static short[] GetOnes(this short value)
    {
        var ones = GetOnes((ushort) value);
        var num = ones.Length;
        var array = new short[num];
        for (var i = 0; i < num; i++)
        {
            array[i] = (short) ones[i];
        }

        return array;
    }

    public static int[] GetOnes(this int value)
    {
        var ones = ((uint) value).GetOnes();
        var num = ones.Length;
        var array = new int[num];
        for (var i = 0; i < num; i++)
        {
            array[i] = (int) ones[i];
        }

        return array;
    }

    public static long[] GetOnes(this long value)
    {
        var ones = GetOnes((ulong) value);
        var num = ones.Length;
        var array = new long[num];
        for (var i = 0; i < num; i++)
        {
            array[i] = (long) ones[i];
        }

        return array;
    }

    public static int CompareTimestamp(this byte[] @this, byte[] other)
    {
        var num = @this.Length;
        if (other.Length != num)
        {
            throw new NotSupportedException();
        }

        for (var i = 0; i < num; i++)
        {
            if (@this[i] > other[i])
            {
                return 1;
            }

            if (@this[i] < other[i])
            {
                return -1;
            }
        }

        return 0;
    }

    public static long ToInt64WithBigEndian(this byte[] timestamp)
    {
        var num = (timestamp[0] << 24) | (timestamp[1] << 16) | (timestamp[2] << 8) | timestamp[3];
        var num2 = (timestamp[4] << 24) | (timestamp[5] << 16) | (timestamp[6] << 8) | timestamp[7];
        return ((long) num << 32) | (uint) num2;
    }
}