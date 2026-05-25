public class Helpers {
    public static bool IsNumeric(object obj) {
        return obj is byte or sbyte
            or short or ushort
            or int or uint
            or long or ulong
            or float or double
            or decimal;
    }
}