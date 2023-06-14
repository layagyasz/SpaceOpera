namespace SpaceOpera
{
    class StringUtils
    {
        public static char FormatEnumChar(string EnumString)
        {
            return char.ToUpper(EnumString[0]);
        }

        public static string FormatEnumString(string EnumString)
        {
            return string.Join(" ", EnumString.Split('_').Select(FormatCase));
        }

        public static string FormatCase(string Value)
        {
            if (Value.Length == 0)
            {
                return "";
            }
            return char.ToUpper(Value[0]) + Value.Substring(1).ToLower();
        }
    }
}