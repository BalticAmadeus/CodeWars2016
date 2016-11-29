using System.Collections.Generic;

namespace Game.AdminClient.Utilities
{
    public static class PasswordEncoder
    {
        private static readonly IDictionary<char, char> FirstLevelEncodes = new Dictionary<char, char>
        {
            {'p', 'o'}, {'o', 'p'}, {'l', 'i'}, {'i', 'l'}, {'t', 'y'}, {'y', 't'}, {'k', 'a'}, {'a', 'k'}, {'r', 'e'}, {'e', 'r'}, {'m', 'u'}, {'u', 'm'},
            {'P', 'O'}, {'O', 'P'}, {'L', 'I'}, {'I', 'L'}, {'T', 'Y'}, {'Y', 'T'}, {'K', 'A'}, {'A', 'K'}, {'R', 'E'}, {'E', 'R'}, {'M', 'U'}, {'U', 'M'},
            {'0', '1'}, {'1', '0'}, {'8', '2'}, {'2', '8'}, {'3', '4'}, {'4', '3'}, {'9', '6'}, {'6', '9'}, {'7', '5'}, {'5', '7'},
        };

        private static readonly IDictionary<char, char> SecondLevelEncodes = new Dictionary<char, char>
        {
            {'a', 's'}, {'s', 'a'}, {'l', 'y'}, {'y', 'l'}, {'j', 'p'}, {'p', 'j'}, {'t', 'm'}, {'m', 't'}, {'h', 'e'}, {'e', 'h'}, {'v', 'u'}, {'u', 'v'}, {'z', 'b'}, {'b', 'z'},
            {'A', 'S'}, {'S', 'A'}, {'L', 'Y'}, {'Y', 'L'}, {'J', 'P'}, {'P', 'J'}, {'T', 'M'}, {'M', 'T'}, {'H', 'E'}, {'E', 'H'}, {'V', 'U'}, {'U', 'V'}, {'Z', 'B'}, {'B', 'Z'},
        }; 

        public static string Encode(string rawPassword)
        {
            var chars = rawPassword.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (!FirstLevelEncodes.ContainsKey(chars[i]))
                    continue;

                chars[i] = FirstLevelEncodes[chars[i]];
            }

            for (int i = 0; i < chars.Length; i++)
            {
                if (!SecondLevelEncodes.ContainsKey(chars[i]))
                    continue;

                chars[i] = SecondLevelEncodes[chars[i]];
            }

            return new string(chars);
        }

        public static string Decode(string rawPassword)
        {
            var chars = rawPassword.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (!SecondLevelEncodes.ContainsKey(chars[i]))
                    continue;

                chars[i] = SecondLevelEncodes[chars[i]];
            }

            for (int i = 0; i < chars.Length; i++)
            {
                if (!FirstLevelEncodes.ContainsKey(chars[i]))
                    continue;

                chars[i] = FirstLevelEncodes[chars[i]];
            }

            return new string(chars);
        }
    }
}


