using System.Linq;
using System.Collections.Generic;

namespace TownOfPlus
{
    public enum CommandText
    {
        None,
        Name,
        FileName,
        Reset,
        All,
    }

    public enum CommandTag
    {
        None,
        Help,
        SaveSkin,
        LoadSkin,
        DeleteSkin,
        DirectMessage,
        Platform,
        LobbyMaxPlayer,
        Kick,
        Ban,
        Tp,
        SpectatePlayer,
        Tpme,
        Kill,
        Exiled,
        Revive,
        Color,
        CPS,
        DateTime,
        FPS,
        JoinText,
    }

    public static class CommandList
    {

        public static ChatCommandList[] AllCommand =>
            new ChatCommandList[]
            {
                new ChatCommandList(new (){"Help" },
                    true,
                    CommandText.All,
                    "",
                    CommandTag.Help),

                new ChatCommandList(new (){"SaveSkin", "SS"},
                    GameState.IsLobby || GameState.IsFreePlay,
                    CommandText.None,
                    "SaveSkin(SS) [保存するスキン名] : 現在のスキンを保存する",
                    CommandTag.SaveSkin),

                new ChatCommandList(new (){"LoadSkin", "LS"},
                    GameState.IsLobby || GameState.IsFreePlay,
                    CommandText.FileName,
                    "LoadSkin(LS) [スキン名] : 保存したスキンを読み込む",
                    CommandTag.LoadSkin),

                new ChatCommandList(new (){"DeleteSkin", "DS"},
                    GameState.IsLobby || GameState.IsFreePlay,
                    CommandText.FileName,
                    "DeleteSkin(DS) [スキン名] : 保存したスキンを削除する",
                    CommandTag.DeleteSkin),

                new ChatCommandList(new (){"DirectMessage", "DM"},
                    GameState.IsLobby,
                    CommandText.Name,
                    "DirectMessage(DM) [名前] : そのプレイヤーのみにチャットを送ります。",
                    CommandTag.DirectMessage),

                new ChatCommandList(new (){"Platform"},
                    GameState.IsLobby,
                    CommandText.None,
                    "Platform : プレイヤーの機種を表示します",
                    CommandTag.Platform),

                new ChatCommandList(new (){"LobbyMaxPlayer", "LMP"},
                    GameState.IsLobby && GameState.IsHost,
                    CommandText.None,
                    "LobbyMaxPlayer(LMP) [人数(4~15)] : 部屋の最大人数の変更",
                    CommandTag.LobbyMaxPlayer),

                new ChatCommandList(new (){"Kick"},
                    GameState.IsLobby && GameState.IsHost,
                    CommandText.Name,
                    "Kick [名前] : Kickできます",
                    CommandTag.Kick),

                new ChatCommandList(new (){"Ban"},
                    GameState.IsLobby && GameState.IsHost,
                    CommandText.Name,
                    "Ban [名前] : banできます",
                    CommandTag.Ban),

                new ChatCommandList(new (){"Tp"},
                    (GameState.IsGameStart && GameState.IsDead) || GameState.IsFreePlay,
                    CommandText.Name,
                    "Tp [名前] : 特定の人にテレポートします",
                    CommandTag.Tp),

                new ChatCommandList(new (){"SpectatePlayer", "SP"},
                    (GameState.IsGameStart && GameState.IsDead) || GameState.IsFreePlay,
                    CommandText.Name,
                    "SpectatePlayer(SP) [名前] : 特定の人を観戦します",
                    CommandTag.SpectatePlayer),

                new ChatCommandList(new (){"Tpme"},
                    GameState.IsFreePlay,
                    CommandText.Name,
                    "Tpme [名前] : 特定の人を自分にテレポートします",
                    CommandTag.Tpme),

                new ChatCommandList(new (){"Kill"},
                    GameState.IsFreePlay,
                    CommandText.Name,
                    "Kill [名前] : 指定した人をキルします",
                    CommandTag.Kill),

                new ChatCommandList(new (){"Exiled"},
                    GameState.IsFreePlay,
                    CommandText.Name,
                    "Exiled [名前] : 指定した人を追放します",
                    CommandTag.Exiled),

                new ChatCommandList(new (){"Revive"},
                    GameState.IsFreePlay,
                    CommandText.Name,
                    "Revive [名前] : 指定した人を生きかえらせます",
                    CommandTag.Revive),

                new ChatCommandList(new (){"Color"},
                    GameState.IsFreePlay,
                    CommandText.Name,
                    "Color [名前] : 指定した人の色を変更します",
                    CommandTag.Color),

                new ChatCommandList(new (){"JoinText", "JT"},
                    main.SendJoinPlayer.Getbool() && GameState.IsLobby && GameState.IsHost,
                    CommandText.None,
                    "JoinText : 参加時のチャットを送信します。",
                    CommandTag.JoinText),

                new ChatCommandList(new (){"CPS"},
                    main.CPS.Getbool(),
                    CommandText.Reset,
                    "CPS : CPSの位置設定を有効/無効にできます。",
                    CommandTag.CPS),

                new ChatCommandList(new (){"DateTime", "DT"},
                    main.DateTimeSetting.Getbool(),
                    CommandText.Reset,
                    "DateTime(DT) : DateTimeの位置設定を有効/無効にできます。",
                    CommandTag.DateTime),

                new ChatCommandList(new (){"FPS"},
                    main.FPS.Getbool(),
                    CommandText.Reset,
                    "FPS : FPSの位置設定を有効/無効にできます。",
                    CommandTag.FPS),
            };

        public static string ChatComHelp(params CommandTag[] com)
        {
            var help = "\n\n===チャットコマンド===";
            try
            {
                foreach (var args in com)
                {
                    var info = AllCommand.FirstOrDefault(w => w.Tag.Equals(args));
                    help += "\n" + info.Help.SetSize(1.5f);
                }
            }
            catch { }
            return help;
        }

        public static string GetHelpText(bool IsAll = false)
        {
            var HelpText = IsAll ? "===コマンド一覧===" : "===実行可能なコマンド一覧===";
            var ComList = AllCommand.Where(w => w.Terms == true || IsAll);
            if (ComList.Count() == 0) return "実行可能なコマンドはありません";
            foreach (var List in ComList)
            {
                if (List.Help == TextPlus.ComWord.ToString()) continue;
                HelpText += "\n" + List.Help;
            }
            if (!IsAll) HelpText += $"\n「{TextPlus.ComWord}Help All」ですべてのコマンドを表示できます";
            return HelpText;
        }

        public class ChatCommandList
        {
            public List<string> Command;
            public bool Terms;
            public CommandText Type;
            public string Help;
            public CommandTag Tag;

            public ChatCommandList(List<string> command, bool terms, CommandText type, string help, CommandTag tag)
            {
                Command = command.Select(s => TextPlus.ComWord + s).ToList();
                Terms = terms;
                Type = type;
                Help = TextPlus.ComWord + help;
                Tag = tag;
            }
        }
    }
}
