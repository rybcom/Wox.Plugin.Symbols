using System.Collections.Generic;

namespace Wox.Plugin.Symbols
{
    public class SymbolDescriptor
    {
        #region properties

        public string Keyword { get; set; }
        public List<string> Symbols { get; set; }
        public string CharacterSet { get; set; }
        public bool Enabled { get; set; }

        #endregion
    }
}
