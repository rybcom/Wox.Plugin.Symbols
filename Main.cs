using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wox.Plugin;
using mroot_lib;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Wox.Plugin.Symbols
{
    static class Paths
    {
        public static string EtcFolderPath => mroot_lib.Paths.SystemFolders.etc;
        public static string SymbolsrConfigFolder => Path.Combine(EtcFolderPath, "wox_plugins", "symbols");
    }

    public class Main : IPlugin, IReloadable
    {

        #region wox overrides
        public void Init(PluginInitContext context)
        {
            ReloadCommadList();
        }

        public List<Result> Query(Query query)
        {
            List<Result> resultList = new List<Result>();

            AddCommands(resultList, query);
            AddOpenConfigCommand(resultList, query);

            return resultList;
        }

        public void ReloadData()
        {
            ReloadCommadList();
        }

        #endregion

        #region commands

        private void AddOpenConfigCommand(List<Result> resultList, Query query)
        {
            if (StringTools.IsEqualOnStart(query.FirstSearch, "config", "settings"))
            {
                Result command = new Result
                {
                    Title = "Open config folder",
                    SubTitle = "Configuration folder",
                    Score = 5000,
                    IcoPath = "Images\\settings.png",
                    Action = e =>
                    {
                        ProcessStartInfo pinfo = new ProcessStartInfo
                        {
                            FileName = mroot.substitue_enviro_vars("||dcommander||"),
                            Arguments = $"-P L -T {Paths.SymbolsrConfigFolder}"
                        };
                        Process.Start(pinfo);

                        return true;
                    }
                };
                resultList.Add(command);
            }
        }

        private void ReloadCommadList()
        {
            CommandsReader reader = new CommandsReader();
            this._symbols = reader.ReadSymbolsFromConfig(Paths.SymbolsrConfigFolder);
        }

        private void AddCommands(List<Result> resultList, Query query)
        {

            foreach (var symbol in _symbols.Where(x => x.Enabled))
            {
                if (query.Search.Length == 1)
                {
                    if (query.FirstSearch.Equals(symbol.Keyword))
                    {
                        foreach (var symbolCharacter in symbol.Symbols)
                        {
                            Result commandResult = new Result
                            {
                                Title = symbolCharacter,
                                Score = 5000,
                                SubTitle = $"Name : {symbol.Keyword}, Charater set : {symbol.CharacterSet}",
                                IcoPath = "Images\\symbol.png",

                                Action = e =>
                                {
                                    Clipboard.SetText(symbolCharacter);
                                    void execution_sending_keys()
                                    {
                                        Thread.Sleep(100);
                                        SendKeys.SendWait(symbolCharacter);
                                    }
                                    new Task(execution_sending_keys).Start();
                                    return true;
                                }
                            };

                            resultList.Add(commandResult);
                        }
                    }
                }
                else if(StringTools.ContainsAllWords($"{symbol.Keyword} {symbol.CharacterSet}", query.FirstSearch, query.SecondSearch))
                {
                    foreach (var symbolCharacter in symbol.Symbols)
                    {
                        Result commandResult = new Result
                        {
                            Title = symbolCharacter,
                            Score = 5000,
                            SubTitle = $"Name : {symbol.Keyword}, Charater set : {symbol.CharacterSet}",
                            IcoPath = "Images\\symbol.png",

                            Action = e =>
                            {
                                Clipboard.SetText(symbolCharacter);
                                void execution_sending_keys()
                                {
                                    Thread.Sleep(100);
                                    SendKeys.SendWait(symbolCharacter);
                                }
                                new Task(execution_sending_keys).Start();
                                return true;
                            }
                        };

                        resultList.Add(commandResult);
                    }
                }
            }
        }

        #endregion

        #region members

        private List<SymbolDescriptor> _symbols;

        #endregion
    }
}
