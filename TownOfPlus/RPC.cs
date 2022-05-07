using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace TownOfPlus
{

    enum TOPRPC
    {
        TOPVersionShare,
    }

    public static class TOPRPCProcedure
    {

        public static void TOPversionshare(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            System.Version ver;
            if (revision < 0)
                ver = new System.Version(major, minor, build);
            else
                ver = new System.Version(major, minor, build, revision);
            CustomOverlays.playerVersions[clientId] = new CustomOverlays.PlayerVersion(ver, guid);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        class RPCHandlerPatch
        {
            static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte packetId = callId;
                switch (packetId)
                {
                    case (byte)TOPRPC.TOPVersionShare:
                        int major = reader.ReadPackedInt32();
                        int minor = reader.ReadPackedInt32();
                        int patch = reader.ReadPackedInt32();
                        int versionOwnerId = reader.ReadPackedInt32();
                        byte revision = 0xFF;
                        Guid guid;
                        if (reader.Length - reader.Position >= 17)
                        { // enough bytes left to read
                            revision = reader.ReadByte();
                            // GUID
                            byte[] gbytes = reader.ReadBytes(16);
                            guid = new Guid(gbytes);
                        }
                        else
                        {
                            guid = new Guid(new byte[16]);
                        }
                        TOPRPCProcedure.TOPversionshare(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                        break;
                }
            }
        }
    }
}