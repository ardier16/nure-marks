using System.Collections.Generic;

namespace NUREMarks.Models
{
    static class EmailGenerator
    {
        static Dictionary<char, string> dict = new Dictionary<char, string>
        {
            { 'а', "a" },
            { 'б', "b" },
            { 'в', "v" },
            { 'г', "h" },
            { 'ґ', "g" },
            { 'д', "d" },
            { 'е', "e" },
            { 'є', "ie" },
            { 'ж', "zh" },
            { 'з', "z" },
            { 'и', "y" },
            { 'й', "i" },
            { 'і', "i" },
            { 'ї', "ii" },
            { 'к', "k" },
            { 'л', "l" },
            { 'м', "m" },
            { 'н', "n" },
            { 'о', "o" },
            { 'п', "p" },
            { 'р', "r" },
            { 'с', "s" },
            { 'т', "t" },
            { 'у', "u" },
            { 'ф', "f" },
            { 'х', "kh" },
            { 'ц', "ts" },
            { 'ч', "ch" },
            { 'ш', "sh" },
            { 'щ', "shch" },
            { 'ь', "" },
            { 'ю', "iu" },
            { 'я', "ia" },
            { '`', "ia" },
            { '-', "-" },
            { '+', "" }
        };


        public static string GenerateNureEmail(string fullName)
        {
            string firstName = fullName.Split(' ')[1].ToLower();
            string lastName = fullName.Split(' ')[0].ToLower();

            return TranslateString(firstName) + "." + 
                   TranslateString(lastName) + "@nure.ua";
        }

        public static string GetFirstLetter(char letter)
        {
            switch (letter)
            {
                case 'я':
                    return "ya";
                case 'ю':
                    return "yu";
                case 'є':
                    return "ye";
                default:
                    return "";
            }
        }

        public static string TranslateString(string source)
        {
            string result = "";

            for (int i = 0; i < source.Length; i++)
            {
                if (i == 0 && (source[i] == 'я' || source[i] == 'ю'
                    || source[i] == 'є'))
                    result += GetFirstLetter(source[i]);
                else
                    result += dict[source[i]];
            }

            return result;
        }
    }
}
