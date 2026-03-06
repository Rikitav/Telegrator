namespace Telegrator.Analyzers.RoslynExtensions
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpper(this string target)
        {
            char[] chars = target.ToCharArray();
            int index = chars.IndexOf(char.IsLetter);
            chars[index] = char.ToUpper(chars[index]);
            return new string(chars);
        }

        public static string FirstLetterToLower(this string target)
        {
            char[] chars = target.ToCharArray();
            int index = chars.IndexOf(char.IsLetter);
            chars[index] = char.ToLower(chars[index]);
            return new string(chars);
        }
    }
}
