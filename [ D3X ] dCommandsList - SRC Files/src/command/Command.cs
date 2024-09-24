using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using static dCommandsList.dCommandsList;

namespace dCommandsList
{
    public static class Command
    {
        public static void Load()
        {
            var config = Config.config?.Settings;
            if (config == null) return;

            var commands = new Dictionary<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)>
            {
                { SplitCommands(config.Help_Commands), ("Help Command", Command_Help) }
            };

            foreach (var commandPair in commands)
            {
                foreach (var command in commandPair.Key)
                {
                    Instance.AddCommand($"css_{command}", commandPair.Value.description, commandPair.Value.handler);
                }
            }
        }

        private static IEnumerable<string> SplitCommands(string commands)
        {
            return commands.Split(',').Select(c => c.Trim());
        }

        public static void AddCommands(IEnumerable<string> commands, string description, CommandInfo.CommandCallback commandAction)
        {
            foreach (var command in commands)
            {
                Instance.AddCommand($"css_{command}", description, commandAction);
            }
        }

        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public static void Command_Help(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            string commandsFilePath = Path.Combine(Instance.ModuleDirectory, "commands.txt");
            EnsureCommandsFileExists(commandsFilePath);

            var displayMode = Config.config.Settings.DisplayMode;
            if (displayMode == "console" || displayMode == "both")
            {
                ShowConsoleHelp(player);
            }

            if (displayMode == "menu" || displayMode == "both")
            {
                ShowMenuHelp(player);
            }
        }

        private static void EnsureCommandsFileExists(string filePath)
        {
            if (File.Exists(filePath)) return;

            var commandsContent = 
                "# Console\n" +
                "=======================================================================================\n" +
                "============================== KOMENDY NA SERWERZE ====================================\n" +
                "=======================================================================================\n" +
                "!sklep                         - otwiera sklep\n" +
                "!ranga                         - Pokazuje ranking\n" +
                "!pomoc                         - Wyświetla to menu\n" +
                " \n" +
                "Więcej komend znajdziesz w menu wyświetlonym z lewej strony ekranu!\n" +
                "Zapraszamy na CS-Zjarani.pl\n" +
                "=======================================================================================\n" +
                "=======================================================================================\n" +
                "=======================================================================================\n" +
                "# Menu\n" +
                "!sklep|Otwiera sklep\n" +
                "!ranga|Pokazuje ranking\n" +
                "!pomoc|Wyświetla to menu\n";

            File.WriteAllText(filePath, commandsContent);
        }

        private static void ShowConsoleHelp(CCSPlayerController player)
        {
            string[] lines = File.ReadAllLines(Path.Combine(Instance.ModuleDirectory, "commands.txt"));
            bool inConsoleSection = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("# Console"))
                {
                    inConsoleSection = true;
                    continue;
                }

                if (line.StartsWith("# Menu")) break;

                if (inConsoleSection)
                {
                    player.PrintToConsole(line);
                }
            }

            player.PrintToChat($" {ChatColors.DarkRed}► {ChatColors.Green}[{ChatColors.DarkRed} POMOC {ChatColors.Green}] {ChatColors.Green}✔ {ChatColors.Lime}Sprawdź wynik w konsoli.");
        }

        private static void ShowMenuHelp(CCSPlayerController player)
        {
            string[] lines = File.ReadAllLines(Path.Combine(Instance.ModuleDirectory, "commands.txt"));
            var menuCommands = new List<(string command, string description)>();

            bool inMenuSection = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("# Menu")) 
                {
                    inMenuSection = true;
                    continue;
                }

                if (inMenuSection)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        menuCommands.Add((parts[0].Trim(), parts[1].Trim()));
                    }
                }

                var menu = Instance._api.NewMenu($"<font color='{Config.config.Settings.Menu_Title_Color}'>{Config.config.Settings.Menu_Title}</font>");

                foreach (var (command, description) in menuCommands)
                {
                    menu.AddMenuOption($"» <font color='#ffd500'>{command}</font>", (player, option) =>
                    {
                        if (Config.config.Settings.Menu_Close_After_Select)
                        {
                            Instance._api.CloseMenu(player);
                        }

                        player.PrintToChat($" {ChatColors.DarkRed}► {ChatColors.Green}[{ChatColors.DarkRed} POMOC {ChatColors.Green}] {ChatColors.Green}✔ {ChatColors.Lime}Komenda: {ChatColors.DarkRed}{command} {ChatColors.Lime}- {ChatColors.DarkRed}{description}");
                    }, false);
                }

                menu.Open(player);
            }
        }
    }
}