using MikuSB.Proto;

namespace MikuSB.GameServer.Server.Packet.Recv.Login;

[Opcode(CmdIds.ReqRename)]
public class HandlerReqRename : Handler
{
    public override async Task OnHandle(Connection connection, byte[] data, ushort seqNo)
    {
        await connection.SendPacket(CmdIds.RspRename);
    }
}
