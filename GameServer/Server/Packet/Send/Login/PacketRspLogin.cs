using MikuSB.GameServer.Game.Player;
using MikuSB.TcpSharp;
using MikuSB.Proto;
using MikuSB.Util.Extensions;

namespace MikuSB.GameServer.Server.Packet.Send.Login;

public class PacketRspLogin : BasePacket
{
    public PacketRspLogin(PlayerInstance player) : base(CmdIds.RspLogin)
    {
        var proto = new RspLogin
        {
            Timestamp = (uint)Extensions.GetUnixSec(),
            WorldChannel = 1,
            AreaId = 1,
            Data = player.Data.ToProto(),
            NeedRename = false
        };

        SetData(proto);
    }
}
