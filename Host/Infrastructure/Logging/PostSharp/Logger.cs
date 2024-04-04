namespace Host.Infrastructure.Logging.PostSharp
{
    public static class Logger
    {
        private static int indentLevel;

        public static void Indent()
        {
            indentLevel++;
        }

        public static void Unindent()
        {
            indentLevel--;
        }

        public static void WriteLine(string message)
        {
            Console.Write(new string(' ', 3 * indentLevel));
            Console.WriteLine(message);
        }
    }
}
