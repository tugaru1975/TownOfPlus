using System;
using System.Collections.Generic;
using System.Text;

namespace TownOfPlus
{
    public static class CommandList
    {
        public enum CommandText
        {
            None,
            Name,
            MapName,
            FileName,
            Platforms,
            Reset
        }
        public static ChatCommandList[] AllCommand()
        {
            var ChatCommandList = new ChatCommandList[]
            {
                new ChatCommandList(new string[]{"/Help" },
                    true,
                    (int)CommandText.None,
                    ""),

                new ChatCommandList(new string[]{"/SaveSkin", "/SS"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined || AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.None,
                    "/SaveSkin(SS) [保存するスキン名] : 現在のスキンを保存する"),

                new ChatCommandList(new string[]{"/LoadSkin", "/LS"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined || AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.FileName,
                    "/LoadSkin(LS) [スキン名] : 保存したスキンを読み込む"),

                new ChatCommandList(new string[]{"/DeleteSkin", "/DS"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined || AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.FileName,
                    "/DeleteSkin(DS) [スキン名] : 保存したスキンを削除する"),

                new ChatCommandList(new string[]{ "/DirectMessage", "/DM"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/DirectMessage(DM) [名前] : そのプレイヤーのみにチャットを送ります。"),

                new ChatCommandList(new string[]{"/LobbyMaxPlayer", "/LMP"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.None,
                    "/LobbyMaxPlayer(LMP) [人数(4~15)] : 部屋の最大人数の変更"),

                new ChatCommandList(new string[]{"/Kick"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Kick [名前] : Kickできます"),

                new ChatCommandList(new string[]{"/Ban"},
                    AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Ban [名前] : banできます"),

                new ChatCommandList(new string[]{"/Tp"},
                    (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.Data.IsDead) || AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Tp [名前] : 特定の人にテレポートします"),

                new ChatCommandList(new string[]{"/SpectatePlayer", "/SP"},
                    (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.Data.IsDead) || AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/SpectatePlayer(SP) [名前] : 特定の人を観戦します"),

                new ChatCommandList(new string[]{"/Tpme"},
                    AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Tpme [名前] : 特定の人を自分にテレポートします"),

                new ChatCommandList(new string[]{"/Kill"},
                    AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Kill [名前] : 指定した人をキルします"),

                new ChatCommandList(new string[]{"/Exiled"},
                    AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Exiled [名前] : 指定した人を追放します"),

                new ChatCommandList(new string[]{"/Revive"},
                    AmongUsClient.Instance.GameMode == GameModes.FreePlay,
                    (int)CommandText.Name,
                    "/Revive [名前] : 指定した人を生きかえらせます"),

                new ChatCommandList(new string[]{"/ChangeGameName", "/CGN"},
                    main.ChangeGameName.Value && ((AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost) || AmongUsClient.Instance.GameMode == GameModes.FreePlay),
                    (int)CommandText.None,
                    "/ChangeGameName(CGN) [変更したい名前(10文字)] : ゲーム中の名前を変えられます"),

                new ChatCommandList(new string[]{"/RandomMap", "/RM"},
                    main.RandomMaps.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.MapName,
                    "/RandomMap(RM) [マップ名] : 選択したものからランダムにマップを選ぶ" +
                                            "\n===マップコマンド一覧===" +
                                            "\nマップ名 [コマンド]" +
                                            "\nTheSkeld [Skeld(S)]" +
                                            "\nMIRA HQ [MIRA(MH)]" +
                                            "\nPolus [Polus(P)]" +
                                            "\nAirShip [AirShip(AS)]"),

                new ChatCommandList(new string[]{"/ChangeLobbyCode", "/CLC"},
                    main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.GameMode == GameModes.OnlineGame,
                    (int)CommandText.None,
                    "/ChangeLobbyCode(CLC) [任意の名前] : ロビーコードの名前を変更できます"),

                new ChatCommandList(new string[]{"/ChangeCodeColor", "/CCC"},
                    main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.GameMode == GameModes.OnlineGame,
                    (int)CommandText.None,
                    "/ChangeCodeColor(CCC) [HEXコード] : ロビーコードの色を変更できます"),

                new ChatCommandList(new string[]{"/SendChat", "/SC"},
                    main.SendJoinPlayer.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.None,
                    "/SendChat(SC) [送るチャット] : 指定したチャットを参加者に送ります"),

                new ChatCommandList(new string[]{"/DoubleName", "/DN"},
                    main.DoubleName.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.None,
                    "/DoubleName(DN) [任意の名前] : 名前に二段目を追加します"),

                new ChatCommandList(new string[]{"/TranslucentName", "/TN"},
                    main.TranslucentName.Value,
                    (int)CommandText.None,
                    "/TranslucentName(TN) [数値] : 名前の透明度を変更します"),

                new ChatCommandList(new string[]{"/OPkick"},
                    main.OPkick.Value && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay,
                    (int)CommandText.Platforms,
                    "/OPkick [機種(数値)] : 追い出すプレイヤーの機種を変更します。"),

                new ChatCommandList(new string[]{"/CPS"},
                    main.CPS.Value,
                    (int)CommandText.Reset,
                    "/CPS : CPSの位置設定を有効/無効にできます。"),

                new ChatCommandList(new string[]{"/DateTime", "/DT"},
                    main.DateTimeSetting.Value,
                    (int)CommandText.Reset,
                    "/DateTime(DT) : DateTimeの位置設定を有効/無効にできます。"),

                new ChatCommandList(new string[]{"/TranslucentChat", "/TC"},
                    main.TranslucentChat.Value,
                    (int)CommandText.None,
                    "/TranslucentChat(TC) [数値] : チャットの透明度を変更します"),

                new ChatCommandList(new string[]{"/FPS"},
                    main.FPS.Value,
                    (int)CommandText.Reset,
                    "/FPS : FPSの位置設定を有効/無効にできます。"),
            };

            return ChatCommandList;
        }
        public class ChatCommandList
        {
            public string[] Command;
            public bool Terms;
            public int Type;
            public string Help;
            public ChatCommandList(string[] command, bool terms, int type, string help)
            {
                Command = command;
                Terms = terms;
                Type = type;
                Help = help;
            }
        }
    }
}
