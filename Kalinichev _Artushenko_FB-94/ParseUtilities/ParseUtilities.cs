using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using static System.Console;

namespace ParseUtilities
{
    public static class StringExtensions
    {
        public static (int min, int max) ParseSection(this string section)
        {
            Regex sectionTemplate = new (@"(\s|\t){0,}\[(\s|\t){0,}(\d){1,}(\s|\t){0,},(\s|\t){0,}(\d){1,}(\s|\t){0,}\](\s|\t){0,}$");
            if(sectionTemplate.IsMatch(section))
            {
                string[] limits = section.Replace("[","").Replace("]","").Replace(" ","").Split(',');
                (int min, int max) = (int.Parse(limits[0]), int.Parse(limits[1]));
                
                if(min >= max) return (0,0);
                else return (min, max);
            }
            else
            {
                throw new FormatException("String was not in the correct format.");
            }
        }
    }

    public static class Extensions
    {
        [Flags]
        public enum ParseStatus
        {
            SUCCESS = 0b_1000_0000,
            FAIL = 0b_0100_0000,
            UNKNOWN = 0b_0010_0000,
            SEARCH_NORMAL = 0b_0000_1000,
            SEARCH_LEFT_OF = 0b_0000_0100,
            SEARCH_INTERSECTS = 0b_0000_0010,
            SEARCH_CONTAINS = 0b_0000_0001,
            MASK = 0b_1111_1111
        }

        public static ParseStatus CheckSyntax(string cmdStr) //check on Regexes
        {
            string cmdStrForRegex = (cmdStr.TrimStart('\t', ' ')).ToUpper();
            string commandTitle = "";
            char[] charsToStopOn = {'\t', '\"', ' ', ';'};

            for (int index = 0; (index < cmdStrForRegex.Length)
                             && (charsToStopOn.Count(x => x == cmdStrForRegex[index]) == 0); index++)
            {
                commandTitle += cmdStrForRegex[index];
            }

            Regex regTemplate;

            switch (commandTitle)
            {
                case "MENU": //ok
                    {
                        regTemplate = new Regex(@"^MENU(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        else break;
                    }

                case "LIST_TREES": //ok
                    {
                        regTemplate = new Regex(@"^LIST_TREES(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        else break;
                    }

                case "CREATE": //ok
                    {
                        regTemplate = new Regex(@"^CREATE(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;

                        break;
                    }

                case "INSERT"://ok
                    {
                        regTemplate = new Regex(@"^INSERT(\s|\t){1,}(\w){1,}(\s|\t){1,}\[(\s|\t){0,}(\d){1,}(\s|\t){0,},(\s|\t){0,}(\d){1,}(\s|\t){0,}\](\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;

                        else break;
                    }

                case "CONTAINS"://ok
                    {
                        regTemplate = new Regex(@"^CONTAINS(\s|\t){1,}(\w){1,}(\s|\t){1,}\[(\s|\t){0,}(\d){1,}(\s|\t){0,},(\s|\t){0,}(\d){1,}(\s|\t){0,}\](\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;

                        break;
                    }

                case "PRINT_TREE": //ok
                    {
                        regTemplate = new Regex(@"^PRINT_TREE(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        
                        break;
                    }

                case "SEARCH": //ok
                    {
                        regTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SEARCH_NORMAL | ParseStatus.SUCCESS;
                        
                        regTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){1,}WHERE(\s|\t){1,}CONTAINS(\s|\t){1,}\[(\s|\t){0,}(\d){1,}(\s|\t){0,},(\s|\t){0,}(\d){1,}(\s|\t){0,}\](\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SEARCH_CONTAINS | ParseStatus.SUCCESS;
                        
                        regTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){1,}WHERE(\s|\t){1,}INTERSECTS(\s|\t){1,}\[(\s|\t){0,}(\d){1,}(\s|\t){0,},(\s|\t){0,}(\d){1,}(\s|\t){0,}\](\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SEARCH_INTERSECTS | ParseStatus.SUCCESS;
                        
                        regTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){1,}WHERE(\s|\t){1,}LEFT_OF(\s|\t){1,}(\d){1,}(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SEARCH_LEFT_OF | ParseStatus.SUCCESS;

                        break;
                    }

                case "PC":
                    {
                        regTemplate = new Regex(@"^(\s|\t){0,}PC(\s|\t){1,}(\s|\t|\w|""|,){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        
                        break;
                    }

                case "CLEAR": //ok
                    {
                        regTemplate = new Regex(@"^(\s|\t){0,}CLEAR(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        else break;
                    }

                case "QUIT":
                    {
                        regTemplate = new Regex(@"^(\s|\t){0,}QUIT(\s|\t){0,};$");
                        if (regTemplate.IsMatch(cmdStrForRegex)) return ParseStatus.SUCCESS;
                        else break;
                    }
                
                default:
                {
                    return ParseStatus.UNKNOWN;
                }
            }
            return ParseStatus.FAIL;
        }

        public static void getCommandStr(out string cmdStr)
        {
            string nonPermitedChars = "\\+^%(){}|/\"'$#@!";
            cmdStr = string.Empty;
            bool checkStatus = false;
            do
            {
                Write("> ");

                cmdStr = ReadLine();
                while (!cmdStr.Contains(";"))
                {
                    cmdStr += ReadLine();
                }
                
                if (cmdStr.Length > (cmdStr.IndexOf(';') + 1))
                {
                    cmdStr = cmdStr.Remove(cmdStr.IndexOf(';') + 1);
                }

                if (((cmdStr.Count(ch => (ch == '[') || (ch == ']')) % 2) != 0))
                {
                    WriteLine("Bad syntax: '[', ']' characters must be paired.");
                    continue;
                }

                foreach (char ch in nonPermitedChars)
                {
                    if (cmdStr.Contains(ch))
                    {
                        WriteLine("Bad syntax: non-permissive char: [{0}].", ch);
                        break;
                    }
                    else
                    {
                        if (ch == '\\') checkStatus = true;
                        else continue;
                    }
                }
            }
            while (!checkStatus);
        }     

        public static void splitCommandToWordsList(ref string cmdStr, ref List<string> cmdArgsList)
        {
            string word = string.Empty;
            for (int indexOfStr = 0; indexOfStr < cmdStr.Length;)
            {
                skipWhitespace(ref cmdStr, ref indexOfStr);
                if (indexOfStr >= cmdStr.Length) break;
                if (cmdStr[indexOfStr] == '[')
                {
                    word += cmdStr[indexOfStr];
                    indexOfStr++;
                    while (cmdStr[indexOfStr] != ']')
                    {
                        word += cmdStr[indexOfStr];
                        indexOfStr++;
                    }
                    word += cmdStr[indexOfStr];
                    indexOfStr++;
                }
                else
                {
                    while (cmdStr[indexOfStr] != ' ')
                    {
                        word += cmdStr[indexOfStr];
                        indexOfStr++;
                        if (indexOfStr == cmdStr.Length) break;
                    }
                }
                if (!(word.Contains('[') && word.Contains(']') && (word.Length == 2)))
                    cmdArgsList.Add(new string(word));
                word = string.Empty;
            }
            
            static void skipWhitespace(ref string str, ref int index) //skip to non-blank/whitespace index
            {
                while (str[index] == ' ' || str[index] == '\t' || str[index] == '\r')
                {
                    index++;
                    if (index == str.Length) break;
                }
            }
        }
    }
}
