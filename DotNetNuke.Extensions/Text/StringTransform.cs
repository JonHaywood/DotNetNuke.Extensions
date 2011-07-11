using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DotNetNuke.Extensions.Text
{
    /// <summary>
    /// Provides transforms to a string that .NET does not provide natively.
    /// </summary>
    public class StringTransform
    {
        #region Public Properties
        /// <summary>
        /// StringBuilder used by this objects.
        /// </summary>
        public StringBuilder Builder { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public StringTransform()
        {
            Builder = new StringBuilder();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">String to use in transformations.</param>
        public StringTransform(string value)
        {
            Builder = new StringBuilder(value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">StringBuilder containing string to use in transformations.</param>
        public StringTransform(StringBuilder builder)
        {
            Check.Ensure(builder != null, "Builder must not be null");
            Builder = builder;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="DotNetNuke.Extensions.Text.StringTransform"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator StringTransform(string s)
        {
            return new StringTransform(s);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Base64 encodes a string.
        /// </summary>        
        /// <returns>A base64 encoded string</returns>
        public string EncodeToBase64()
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(Builder.ToString());
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64 decodes a string.
        /// </summary>
        /// <returns>A decoded string</returns>
        public string DecodeFromBase64()
        {
            byte[] decbuff = Convert.FromBase64String(Builder.ToString());
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>        
        /// <param name="newValue">The value to replace.</param>
        /// <param name="oldValue">The new value to be inserted</param>
        /// <returns>A string</returns>
        public string CaseInsenstiveReplace(string newValue, string oldValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(Builder.ToString(), newValue);
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters. The replace is NOT case
        /// sensitive.
        /// </summary>        
        /// <param name="filterWords">The words to repace in the input string.</param>
        /// <returns>A string.</returns>
        public string FilterWords(params string[] filterWords)
        {
            return FilterWords(char.MinValue, filterWords);
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters. The replace is NOT case
        /// sensitive.
        /// </summary>        
        /// <param name="mask">A character that is inserted for each letter of the replaced word.</param>
        /// <param name="filterWords">The words to repace in the input string.</param>
        /// <returns>A string.</returns>
        public string FilterWords(char mask, params string[] filterWords)
        {
            string stringMask = mask == char.MinValue ? string.Empty : mask.ToString();
            string totalMask = stringMask;
            string input = Builder.ToString();

            foreach (string s in filterWords)
            {
                Regex regEx = new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < s.Length; i++)
                        totalMask += stringMask;
                }

                input = regEx.Replace(input, totalMask);

                totalMask = stringMask;
            }

            return input;
        }

        /// <summary>
        /// Checks the passed string to see if has any of the passed words. Not case-sensitive.
        /// </summary>
        /// <param name="hasWords">The words to check for.</param>
        /// <returns>A collection of the matched words.</returns>
        public MatchCollection HasWords(params string[] hasWords)
        {
            StringBuilder sb = new StringBuilder(hasWords.Length + 50);
            //sb.Append("[");

            foreach (string s in hasWords)
            {
                sb.AppendFormat("({0})|", HtmlSpecialEntitiesEncode(s.Trim()));
            }

            string pattern = sb.ToString();
            pattern = pattern.TrimEnd('|'); // +"]";

            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Matches(Builder.ToString());
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="input">The string to be encoded</param>
        /// <returns>An encoded string</returns>
        public static string HtmlSpecialEntitiesEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlEncode
        /// </summary>        
        /// <returns>An encoded string</returns>
        public string HtmlSpecialEntitiesEncode()
        {
            return HtmlSpecialEntitiesEncode(Builder.ToString());
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="input">The string to be decoded</param>
        /// <returns>The decode string</returns>
        public static string HtmlSpecialEntitiesDecode(string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlDecode
        /// </summary>        
        /// <returns>The decode string</returns>
        public string HtmlSpecialEntitiesDecode()
        {
            return HtmlSpecialEntitiesDecode(Builder.ToString());
        }

        /// <summary>
        /// MD5 encodes the passed string
        /// </summary>
        /// <returns>An encoded string.</returns>
        public string MD5()
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Builder.ToString()));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verified a string against the passed MD5 hash.
        /// </summary>        
        /// <param name="hash">The hash to compare against.</param>
        /// <returns>True if the input and the hash are the same, false otherwise.</returns>
        public bool MD5Verify(string hash)
        {
            // Hash the input.
            string hashOfInput = MD5();

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Left pads the passed input using the HTML non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>        
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadLeftHtmlSpaces(int totalSpaces)
        {
            string space = "&nbsp;";
            return PadLeft(space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadLeft(string pad, int totalWidth)
        {
            return PadLeft(pad, totalWidth, false);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will cut-off the pad so that  
        /// the string does not exceed the total width.
        /// </summary>        
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadLeft(string pad, int totalWidth, bool cutOff)
        {
            string input = Builder.ToString();
            if (input.Length >= totalWidth)
                return input;

            int padCount = pad.Length;
            string paddedString = input;

            while (paddedString.Length < totalWidth)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth);

            return paddedString;
        }

        /// <summary>
        /// Right pads the passed input using the HTML non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>        
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadRightHtmlSpaces(int totalSpaces)
        {
            string space = "&nbsp;";
            return PadRight(space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>        
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadRight(string pad, int totalWidth)
        {
            return PadRight(pad, totalWidth, false);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will cut-off the pad so that  
        /// the string does not exceed the total width.
        /// </summary>        
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to pad the string.</param>
        /// <returns>A padded string.</returns>
        public string PadRight(string pad, int totalWidth, bool cutOff)
        {
            string input = Builder.ToString();
            if (input.Length >= totalWidth)
                return input;

            string paddedString = string.Empty;

            while (paddedString.Length < totalWidth - input.Length)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth - input.Length);

            paddedString += input;

            return paddedString;
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>        
        /// <returns>A string</returns>
        public string RemoveNewLines()
        {
            return RemoveNewLines(false);
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>        
        /// <param name="addSpace">If true, adds a space (" ") for each newline and carriage
        /// return found.</param>
        /// <returns>A string</returns>
        public string RemoveNewLines(bool addSpace)
        {
            string replace = string.Empty;
            if (addSpace)
                replace = " ";

            string pattern = @"[\r|\n]";
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return regEx.Replace(Builder.ToString(), replace);
        }

        /// <summary>
        /// Reverse a string.
        /// </summary>        
        /// <returns>A string</returns>
        public string Reverse()
        {
            string input = Builder.ToString();
            if (input.Length <= 1)
                return input;

            char[] c = input.ToCharArray();
            StringBuilder sb = new StringBuilder(c.Length);
            for (int i = c.Length - 1; i > -1; i--)
                sb.Append(c[i]);

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to sentence case.
        /// </summary>        
        /// <returns>A string</returns>
        public string SentenceCase()
        {
            string input = Builder.ToString();
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() + sentence.Substring(1);
        }

        /// <summary>
        /// Converts all spaces to HTML non-breaking spaces
        /// </summary>        
        /// <returns>A string</returns>
        public string SpaceToNbsp()
        {
            string space = "&nbsp;";
            return Builder.ToString().Replace(" ", space);
        }

        /// <summary>
        /// Performs basically the same job as String.Split, but does 
        /// trim all parts, no empty parts are returned, e.g. 
        /// "hi  there" returns "hi", "there", String.Split would return 
        /// "hi", "", "there". 
        /// </summary>         
        /// <param name="separator">Character that separates tokens.</param>
        public string[] SplitAndTrim(char separator)
        {
            List<string> ret = new List<string>();
            string[] splitted = Builder.ToString().Split(new char[] { separator });
            foreach (string s in splitted)
                if (s.Length > 0)
                    ret.Add(s);
            return ret.ToArray();
        }

        /// <summary>
        /// Splits the string into a collection of items. Uses the built-in 
        /// Convert methods to change a string to the provided type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator"></param>
        /// <returns>Collections.</returns>
        public IEnumerable<T> Split<T>(char separator)
        {            
            Type targetType = typeof(T);
            return Split(separator, s => (T)Convert.ChangeType(s, targetType));                
        }

        /// <summary>
        /// Splits the string into a collection of items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator">Character delimiter.</param>
        /// <returns>Collection.</returns>
        public IEnumerable<T> Split<T>(char separator, Func<string, T> conversionFunc)
        {
            string[] splitted = Builder.ToString().Split(separator);
            Type targetType = typeof(T);
            foreach (string s in splitted)
                yield return conversionFunc(s);
        }

        /// <summary>
        /// Removes all HTML tags from the passed string
        /// </summary>        
        /// <returns>A string.</returns>
        public string StripTags()
        {
            Regex stripTags = new Regex("<(.|\n)+?>");
            return stripTags.Replace(Builder.ToString(), "");
        }

        /// <summary>
        /// Strip Non Valid XML Characters.
        /// </summary>        
        /// <returns>A string.</returns>
        public string StripNonValidXMLCharacters()
        {
            string s = Builder.ToString();
            StringBuilder _validXML = new StringBuilder(s.Length, s.Length); // Used to hold the output.
            char[] charArray = s.ToCharArray();

            if (string.IsNullOrEmpty(s)) return string.Empty; // vacancy test.

            for (int i = 0; i < charArray.Length; i++)
            {
                char current = charArray[i];
                if ((current == 0x9) ||
                (current == 0xA) ||
                (current == 0xD) ||
                ((current >= 0x20) && (current <= 0xD7FF)) ||
                ((current >= 0xE000) && (current <= 0xFFFD)))
                    _validXML.Append(current);
            }
            return _validXML.ToString();
        }

        /// <summary>
        /// Strips all whitespace from a string.
        /// </summary>        
        /// <returns>A string.</returns>
        public string StripWhitespace()
        {
            return Regex.Replace(Builder.ToString(), @"\s", "");
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>        
        /// <returns>A string.</returns>
        public string TitleCase()
        {
            return TitleCase(true);
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>        
        /// <param name="ignoreShortWords">If true, does not capitalize words like
        /// "a", "is", "the", etc.</param>
        /// <returns>A string.</returns>
        public string TitleCase(bool ignoreShortWords)
        {
            string input = Builder.ToString();
            List<string> ignoreWords = null;
            if (ignoreShortWords)
            {
                //TODO: Add more ignore words?
                ignoreWords = new List<string>();
                ignoreWords.Add("a");
                ignoreWords.Add("is");
                ignoreWords.Add("was");
                ignoreWords.Add("the");
            }

            string[] tokens = input.Split(' ');
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (string s in tokens)
            {
                if (ignoreShortWords == true
                    && s != tokens[0]
                    && ignoreWords.Contains(s.ToLower()))
                {
                    sb.Append(s + " ");
                }
                else
                {
                    sb.Append(s[0].ToString().ToUpper());
                    sb.Append(s.Substring(1).ToLower());
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Removes multiple spaces between words
        /// </summary>        
        /// <returns>A string.</returns>
        public string TrimIntraWords()
        {
            Regex regEx = new Regex(@"[\s]+");
            return regEx.Replace(Builder.ToString(), " ");
        }

        /// <summary>
        /// Converts new line(\n) and carriage return(\r) symbols to
        /// HTML line breaks.
        /// </summary>        
        /// <returns>A string.</returns>
        public string NewLineToBreak()
        {
            Regex regEx = new Regex(@"[\n|\r]+");
            return regEx.Replace(Builder.ToString(), "<br />");
        }

        /// <summary>
        /// Wraps the passed string up the 
        /// until the next whitespace on or after the total charCount has been reached
        /// for that line.  Uses the environment new line
        /// symbol for the break text.
        /// </summary>        
        /// <param name="charCount">The number of characters per line.</param>
        /// <returns>A string.</returns>
        public string WordWrap(int charCount)
        {
            return WordWrap(charCount, false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cuttOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>        
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <returns>A string.</returns>
        public string WordWrap(int charCount, bool cutOff)
        {
            return WordWrap(charCount, cutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cuttOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the passed breakText
        /// for lineBreaks.
        /// </summary>        
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <param name="breakText">The line break text to use.</param>
        /// <returns>A string.</returns>
        public string WordWrap(int charCount, bool cutOff,
            string breakText)
        {
            string input = Builder.ToString();
            StringBuilder sb = new StringBuilder(input.Length + 100);
            int counter = 0;

            if (cutOff)
            {
                while (counter < input.Length)
                {
                    if (input.Length > counter + charCount)
                    {
                        sb.Append(input.Substring(counter, charCount));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(input.Substring(counter));
                    }
                    counter += charCount;
                }
            }
            else
            {
                string[] strings = input.Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1; // the added one is to represent the inclusion of the space.
                    if (i != 0 && counter > charCount)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }
            return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
        }

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>       
        /// <returns></returns>
        public string EncodeToJs()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("\"");
            foreach (char c in Builder.ToString())
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            //sb.Append("\"");

            return sb.ToString();
        }

        /// <summary>
        /// Replace function that can ignore case.
        /// </summary>
        /// <see cref="http://www.west-wind.com/weblog/posts/60355.aspx"/>        
        /// <param name="FindString">The find string.</param>
        /// <param name="ReplaceString">The replace string.</param>        
        /// <returns>String with values replaced.</returns>
        public string Replace(string OrigString, string FindString, string ReplaceString)
        {
            return Replace(FindString, ReplaceString, true);
        }


        /// <summary>
        /// Replace function that can ignore case.
        /// </summary>
        /// <see cref="http://www.west-wind.com/weblog/posts/60355.aspx"/>        
        /// <param name="FindString">The find string.</param>
        /// <param name="ReplaceString">The replace string.</param>
        /// <param name="CaseInsensitive">if set to <c>true</c> [case insensitive].</param>
        /// <returns>String with values replaced.</returns>
        public string Replace(string FindString, string ReplaceString, bool CaseInsensitive)
        {
            string OrigString = Builder.ToString();
            int at1 = 0;
            while (true)
            {
                if (CaseInsensitive)
                    at1 = OrigString.IndexOf(FindString, at1, OrigString.Length - at1, StringComparison.OrdinalIgnoreCase);
                else
                    at1 = OrigString.IndexOf(FindString, at1);

                if (at1 == -1)
                    return OrigString;

                OrigString = OrigString.Substring(0, at1) + ReplaceString + OrigString.Substring(at1 + FindString.Length);
                at1 += ReplaceString.Length;
            }
        }

        #endregion
    }
}
