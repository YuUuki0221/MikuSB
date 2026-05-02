using MikuSB.Data;
using MikuSB.Database.Player;
using MikuSB.GameServer.Game.Player;
using MikuSB.Proto;

namespace MikuSB.GameServer.Server.CallGS.Handlers.Rogue3D;

internal static class Rogue3DStateHelper
{
    private const uint GroupId = 124;
    private const uint LevelPassStart = 20;
    private const uint UnlockDiff1Sid = LevelPassStart + 1;
    private const uint UnlockDiff2Sid = LevelPassStart + 2;
    private const uint UnlockDiff3Sid = LevelPassStart + 3;
    private const uint UnlockDiff4Sid = LevelPassStart + 4;

    public static NtfSyncPlayer EnsureUnlockState(PlayerInstance player)
    {
        var sync = new NtfSyncPlayer();

        EnsureMinAttr(player, UnlockDiff1Sid, 1, sync);
        EnsureMinAttr(player, UnlockDiff2Sid, 1, sync);
        EnsureMinAttr(player, UnlockDiff3Sid, 1, sync);
        EnsureMinAttr(player, UnlockDiff4Sid, 1, sync);

        foreach (var scienceSid in GetUnlockTalentScienceSids())
        {
            EnsureMinAttr(player, scienceSid, 1, sync);
        }

        return sync;
    }

    private static IEnumerable<uint> GetUnlockTalentScienceSids()
    {
        return GameData.Rogue3DTalentData.Values
            .Select(x => x.UnlockCondition)
            .Where(x => x > 0)
            .Distinct()
            .OrderBy(x => x);
    }

    private static void EnsureMinAttr(PlayerInstance player, uint sid, uint value, NtfSyncPlayer sync, bool overwrite = false)
    {
        var attr = player.Data.Attrs.FirstOrDefault(x => x.Gid == GroupId && x.Sid == sid);
        if (attr == null)
        {
            attr = new PlayerAttr { Gid = GroupId, Sid = sid, Val = value };
            player.Data.Attrs.Add(attr);
            AddSync(player, sync, sid, value);
            return;
        }

        if ((!overwrite && attr.Val >= value) || (overwrite && attr.Val == value))
        {
            return;
        }

        attr.Val = value;
        AddSync(player, sync, sid, value);
    }

    private static void AddSync(PlayerInstance player, NtfSyncPlayer sync, uint sid, uint value)
    {
        sync.Custom[player.ToPackedAttrKey(GroupId, sid)] = value;
        sync.Custom[player.ToShiftedAttrKey(GroupId, sid)] = value;
    }
}
