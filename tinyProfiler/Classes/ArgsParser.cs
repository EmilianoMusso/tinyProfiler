using System.Collections.Generic;

namespace tinyProfiler
{
    public class ArgsParser
    {
        public Dictionary<string, string> Arguments { get; set; }

        /// <summary>
        /// Returns the value of a named argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Argument(string name)
        {
            return this.Arguments.GetValueOrDefault(name);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ArgsParser()
        {
            this.Arguments = new Dictionary<string, string>();
        }

        /// <summary>
        /// Internal method to parse arguments
        /// </summary>
        /// <param name="_arguments"></param>
        private void _Parse(string[] _arguments)
        {
            foreach (var _a in _arguments)
            {
                var _p = _a.Split('=');
                if (_p.Length != 2) continue;

                var _key = _p[0].Trim();
                if (!_key.StartsWith("-")) continue;

                this.Arguments.Add(_key.Replace("-", ""), _p[1].Trim());
            }
        }

        /// <summary>
        /// Parse a string to split arguments and returning parameters dictionary
        /// </summary>
        /// <param name="_arguments"></param>
        /// <returns></returns>
        public Dictionary<string, string> Parse(string _arguments)
        {
            var _args = _arguments.Split(' ');
            this._Parse(_args);
            return this.Arguments;
        }

        /// <summary>
        /// Parse an array of string to split arguments and returning parameters dictionary
        /// </summary>
        /// <param name="_arguments"></param>
        /// <returns></returns>
        public Dictionary<string, string> Parse(string[] _args)
        {
            this._Parse(_args);
            return this.Arguments;
        }
    }
}
