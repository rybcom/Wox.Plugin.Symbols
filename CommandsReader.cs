using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using mroot_lib;

namespace Wox.Plugin.Symbols
{

    public class SymbolSettings
    {
        public string CharacterSet { get; set; }
        public bool Enabled { get; set; }
    }


    public class CommandsReader
    {
        #region api

        public List<SymbolDescriptor> ReadSymbolsFromConfig(string configFolder)
        {

            List<SymbolDescriptor> result = new List<SymbolDescriptor>();
            DirectoryInfo directory = new DirectoryInfo(configFolder);

            FileInfo[] Files = directory.GetFiles("*.ini");

            foreach (FileInfo file in Files)
            {
                List<string> lines = File.ReadAllLines(file.FullName).ToList();
                List<string> trimmedLines = FilterRegularLines(lines);
                result.AddRange(GetSymbolsFromLines(trimmedLines));
            }

            return result;
        }

        #endregion

        #region private methods

        private List<string> FilterRegularLines(List<string> lines)
        {
            List<string> result = new List<string>();

            foreach (string line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.IsNullOrWhitespace())
                {
                    continue;
                }

                if (trimmedLine.StartsWith("#") || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("//"))
                {
                    continue;
                }

                result.Add(trimmedLine);
            }

            return result;
        }

        private List<string> GetSection(string sectionName, List<string> lines)
        {
            List<string> result = new List<string>();

            bool inSettingsSection = false;

            foreach (string line in lines)
            {
                if (line.StartsWith($"[{sectionName}]"))
                {
                    inSettingsSection = true;
                    continue;
                }

                if (line.StartsWith("["))
                {
                    inSettingsSection = false;
                    continue;
                }

                if (inSettingsSection)
                {
                    result.Add(line);
                }
            }

            return result;
        }

        private List<SymbolDescriptor> GetSymbolsFromLines(List<string> lines)
        {
            Settings = GetSymbolSettings(GetSection("settings", lines));

            return GetSymbolsActions(GetSection("symbols", lines));
        }

        private List<string> GetSymbols(string text)
        {
            string[] separtator = { "," };
            return text.Split(separtator, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private List<SymbolDescriptor> GetSymbolsActions(List<string> lines)
        {
            List<SymbolDescriptor> results = new List<SymbolDescriptor>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                string keyword = parts[0].Trim();
                string symbolsText = parts[1].Trim();

                SymbolDescriptor action = new SymbolDescriptor
                {
                    Keyword = keyword,
                    Symbols = GetSymbols(symbolsText),
                    CharacterSet = Settings.CharacterSet,
                    Enabled = Settings.Enabled
                };

                results.Add(action);
            }

            return results;
        }

        private SymbolSettings GetSymbolSettings(List<string> lines)
        {
            SymbolSettings result = new SymbolSettings();

            foreach (var line in lines)
            {
                var splitIndex = line.IndexOf('=');
                var key = line.Substring(0, splitIndex);
                var value = line.Substring(splitIndex + 1, line.Length - splitIndex - 1);

                if (key.Equals("enabled"))
                {
                    result.Enabled = Boolean.Parse(value.Trim());
                }
                else if (key.Equals("character_set"))
                {
                    result.CharacterSet = value.Trim();
                }
            }
            return result;
        }

        #endregion

        #region members
        private SymbolSettings Settings { get; set; }

        #endregion
    }
}
