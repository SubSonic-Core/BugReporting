namespace SubSonic
{
    public static partial class Extensions
    {
        private const string empty = "";

        public static bool IsEnabled(this IFactory factory) => factory.Enabled;
        
        public static bool IsNotEnabled(this IFactory factory) => !factory.Enabled;

        public static string IsNot(this IFactory factory)
        {
            if (factory.IsNotEnabled())
                return "not";
            return empty;
        }
    }
}
