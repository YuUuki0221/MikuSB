using MikuSB.Util.Extensions;
using SqlSugar;

namespace MikuSB.Database.Player;

[SugarTable("Player")]
public class PlayerGameData : BaseDatabaseDataHelper
{
    public string? Name { get; set; } = "";
    public string? Signature { get; set; } = "MikuPS";
    public uint Level { get; set; } = 1;
    public int Exp { get; set; } = 0;
    public long RegisterTime { get; set; } = Extensions.GetUnixSec();
    public long LastActiveTime { get; set; }
    [SugarColumn(IsJson = true)] public List<PlayerAttrs> Attrs { get; set; } = [];

    public static PlayerGameData? GetPlayerByUid(long uid)
    {
        var result = DatabaseHelper.GetInstance<PlayerGameData>((int)uid);
        return result;
    }

    public Proto.Player ToProto()
    {
        var proto = new Proto.Player
        {
            Pid = (ulong)Uid,
            Account = Name,
            Name = Name,
            Level = Level,
        };

        foreach (var x in Attrs)
        {
            uint gid = x.Gid;
            uint sid = x.Sid;
            uint val = x.Val;

            if (gid == 0)
            {
                proto.Attrs[sid] = val;
                continue;
            }

            proto.Attrs[ToPackedAttrKey(gid, sid)] = val;
            proto.Attrs[ToShiftedAttrKey(gid, sid)] = val;
        }

        return proto;
    }

    private static uint ToPackedAttrKey(uint gid, uint sid)
    {
        if (gid == 0)
            return sid;

        return (gid * 10000) + sid;
    }

    private static uint ToShiftedAttrKey(uint gid, uint sid)
    {
        if (gid == 0)
            return sid;

        return (gid << 16) | sid;
    }
}

public class PlayerAttrs
{
    public uint Gid { get; set; }
    public uint Sid { get; set; }
    public uint Val { get; set; }
}