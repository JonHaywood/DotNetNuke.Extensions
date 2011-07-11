using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using DotNetNuke.Extensions.Reflection;
using DotNetNuke.Extensions.Data;

namespace DotNetNuke.Extensions
{
    /// <summary>
    /// Class that encapsulates token data.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="parameters">Parameters.</param>
        public Token(string tokenName, Dictionary<string, string> parameters)
        {
            Name = tokenName;
            Parameters = parameters;
        }

        /// <summary>
        /// Name of the token.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Parameter list for this token.
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Returns a specific parameter.
        /// </summary>
        /// <param name="parameterName">Parameter to return.</param>
        /// <returns>Parameter value.</returns>
        public string this[string parameterName]
        {
            get { return Parameters[parameterName]; }
        }
    }

    /// <summary>
    /// Delegate for token handler.
    /// </summary>
    /// <param name="token">Token to be passed in.</param>
    /// <returns>String.</returns>
    public delegate string TokenizerHandler(Token token);

    /// <summary>
    /// Class that will turn a string containing tokens into a rendered string 
    /// with the tokens replaced with relevant data.
    /// </summary>
    public class Tokenizer
    {
        #region Private Variables
        /// <summary>
        /// Token Handlers.
        /// </summary>
        private Dictionary<string, TokenizerHandler> Handlers = new Dictionary<string, TokenizerHandler>();

        /// <summary>
        /// String for this tokenizer.
        /// </summary>
        private string Content;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public Tokenizer() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="content">String content.</param>
        public Tokenizer(string content)
        {
            Check.Require(!string.IsNullOrEmpty(content), "The content string for Tokenizer cannot be null or empty.");
            Content = content;
        }
        #endregion

        #region Bind Methods
        /// <summary>
        /// Binds a NameValueCollection using keys as token names and values as
        /// the token values.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public void Bind(NameValueCollection collection)
        {
            foreach (string key in collection.AllKeys)
                Bind(key, collection[key]);
        }
       
        /// <summary>
        /// Binds the datareader to tokens using column names as token names and column
        /// values as token values.
        /// </summary>
        /// <param name="dr">Data reader to bind.</param>
        public void Bind(IDataReader dr)
        {
            // first read from datareader if needed - if we can't, throw an exception.
            if (dr.IsClosed)
                Check.Ensure(dr.Read(), "Unable to read from the data reader.");

            for (int i = 0; i < dr.FieldCount; i++)
                Bind(dr.GetName(i), dr.GetValue(i).ToString());
        }

        /// <summary>
        /// Binds the datarow content using column names as tokens.
        /// </summary>
        /// <param name="row">Row to bind.</param>
        public void Bind(DataRow row)
        {
            Check.Require(row.Table != null, "The DataRow must have an associated DataTable.");
            foreach (DataColumn column in row.Table.Columns)
                Bind(column.ColumnName, row[column].ToString());
        }

        /// <summary>
        /// Binds the public properties of the object to tokens of the same name.
        /// </summary>
        /// <param name="obj">Object to bind.</param>
        public void Bind(object obj)
        {
            var dictionary = obj.ToDictionary();
            Bind(dictionary);
        }

        /// <summary>
        /// Binds a IDictionary using keys as token names and values as the token values.
        /// </summary>
        /// <param name="dictionary">Dictionary.</param>
        public void Bind(IDictionary dictionary)
        {
            var enumerator = dictionary.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Current;
                Bind(key.ToString(), dictionary[key].ToString());
            }
        }

        /// <summary>
        /// Binds a dictionary to tokens. Uses the key as the token name and the value as
        /// the token value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Dictionary to bind.</param>
        public void Bind<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            foreach (KeyValuePair<TKey, TValue> item in dictionary)
                Bind(item.Key.ToString(), item.Value.ToString());
        }        

        /// <summary>
        /// Binds a token name to the specified value.
        /// </summary>
        /// <param name="tokenName">Token name to register.</param>
        /// <param name="value">Token value.</param>
        public void Bind(string tokenName, string value)
        {
            Bind(tokenName, t => value);
        }

        /// <summary>
        /// Binds a mapping of token names and handlers. The handlers will be
        /// called when rendering the tokens.
        /// </summary>
        /// <param name="dictionary"></param>
        public void Bind(IDictionary<string, TokenizerHandler> dictionary)
        {
            foreach (var item in dictionary)
            {
                Bind(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Binds a token name to a handler. The handler will be called when 
        /// rendering the token.
        /// </summary>
        /// <param name="tokenName">Token name to register.</param>
        /// <param name="handler">Token handler.</param>
        public void Bind(string tokenName, TokenizerHandler handler)
        {
            Handlers[tokenName] = handler;
        }
        #endregion

        #region Render Methods
        /// <summary>
        /// Renders the tokens in the string as data and returns the new string.
        /// </summary>
        /// <returns>Tokenized string.</returns>
        public string Render()
        {
            return Render(Content);
        }

        /// <summary>
        /// Renders the tokens in the string as data and returns the new string.
        /// </summary>
        /// <param name="content">Content to render.</param>
        /// <returns>Tokenized string.</returns>
        public string Render(string content)
        {
            // make sure we have a string
            Check.Require(!string.IsNullOrEmpty(content), "The content string must not be empty or null.");

            string result = content;

            // loop through all the tokens we have registered
            foreach (string tokenName in Handlers.Keys)
            {
                // replace all instances of the token in the string
                string newToken = FindNextToken(tokenName, result);
                while (string.IsNullOrEmpty(newToken))
                {
                    // create a token
                    Token token = CreateToken(tokenName, newToken);

                    // get the replacement
                    string replacement = Handlers[tokenName](token);

                    // replace the token
                    result = ReplaceToken(tokenName, result, replacement);

                    // get the next token
                    newToken = FindNextToken(tokenName, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Render an entire collection of items.
        /// </summary>
        /// <typeparam name="T">Type of collection.</typeparam>
        /// <param name="content">Content to render.</param>
        /// <param name="collection">Collection of items to apply to content.</param>
        /// <returns>Tokenized string.</returns>
        public static string Render<T>(string content, IEnumerable<T> collection)
        {
            StringBuilder result = new StringBuilder();
            foreach (T item in collection)
            {
                // create a tokenizer, and bind each element
                Tokenizer tokenizer = new Tokenizer(content);
                tokenizer.Bind(item);
                result.Append(tokenizer.Render());
            }

            return result.ToString();
        }
        #endregion

        #region Operators Overloads
        /// <summary>
        /// Performs an implicit conversion from <see cref="DotNetNuke.Extensions.Tokenizer"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Tokenizer t)
        {
            return t.Render();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="DotNetNuke.Extensions.Tokenizer"/>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Tokenizer(string s)
        {
            return new Tokenizer(s);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Removes all handlers.
        /// </summary>
        private void ClearHandlers()
        {
            Handlers.Clear();
        }

        /// <summary>
        /// Creates a token object using the provided token string.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="tokenString">Token string to convert into an object.</param>
        /// <returns>Token.</returns>
        private Token CreateToken(string tokenName, string tokenString)
        {
            return new Token(tokenName, ExtractKeyValuePairs(tokenString));
        }

        /// <summary>
        /// Finds the next token in a string.
        /// </summary>
        /// <param name="token">Token to look for.</param>
        /// <param name="input">String to look in.</param>
        /// <returns>Token if there is one, otherwise null.</returns>
        private string FindNextToken(string token, string input)
        {
            var pattern = string.Format(@"(\[{0}[^\]\r\n]*\])", token);
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = regex.Match(input);
            if (m.Success)
                return m.Groups[0].Value.Trim('[', ']');
            else
                return null;
        }

        /// <summary>
        /// Replaces a token in a string.
        /// </summary>
        /// <param name="token">Token to replace.</param>
        /// <param name="input">String to replace in.</param>
        /// <param name="replacement">Replacement for token.</param>
        /// <returns>Input with token replaced with the replacement string.</returns>
        private string ReplaceToken(string token, string input, string replacement)
        {
            var pattern = string.Format(@"(\[{0}[^\]\r\n]*\])", token);
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Extracts a key value pair from a given string.
        /// </summary>
        /// <param name="input">String to extract from.</param>
        /// <returns>KeyValuePair.</returns>
        private KeyValuePair<string, string> ExtractKeyValuePair(string input)
        {
            var regex = new Regex(@"\s*(\w+)=([^\s]+)\s*");
            var groups = regex.Match(input).Groups;
            if (groups.Count == 3)
                return new KeyValuePair<string, string>(groups[1].Value.ToLower(), groups[2].Value.ToLower());
            else
                throw new ArgumentException(string.Format("Input string {0} is not formatted correctly.", input));
        }

        /// <summary>
        /// Extracts a map of key value pairs from a string.
        /// </summary>
        /// <param name="input">String to extract from.</param>
        /// <returns>Map of KeyValuePairs.</returns>
        private Dictionary<string, string> ExtractKeyValuePairs(string input)
        {
            var regex = new Regex(@"\s*(\w+)=([^\s]+)\s*");
            var dictionary = new Dictionary<string, string>();

            Match m = regex.Match(input);
            while (m.Success)
            {
                string subString = m.Groups[0].ToString();
                var item = ExtractKeyValuePair(subString);
                dictionary.Add(item.Key, item.Value);
                m = m.NextMatch();
            }
            return dictionary;
        }
        #endregion
    }
}
