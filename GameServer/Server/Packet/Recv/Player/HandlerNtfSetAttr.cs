using MikuSB.Database.Player;
using MikuSB.Proto;

namespace MikuSB.GameServer.Server.Packet.Recv.Login;

[Opcode(CmdIds.NtfSetAttr)]
public class HandlerNtfSetAttr : Handler
{
    public override async Task OnHandle(Connection connection, byte[] data, ushort seqNo)
    {
        var req = NtfSetAttr.Parser.ParseFrom(data);
        var player = connection.Player!;
        var attr = player.Data.Attrs
            .FirstOrDefault(x => x.Gid == req.Gid && x.Sid == req.Sid);
        if (attr != null) attr.Val = req.Val;
        else
        {
            player.Data.Attrs.Add(new PlayerAttrs
            {
                Gid = req.Gid,
                Sid = req.Sid,
                Val = req.Val
            });
        }
        await player.OnHeartBeat();
    }
}
