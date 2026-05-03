using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MikuSB.GameServer.Server.CallGS.Handlers.House;

[HouseFunc("ChangeNpcSuit")]
public class ChangeNpcSuit : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var req = JsonSerializer.Deserialize<NpcSuitParam>(param);
        var rsp = new JsonObject
        {
            ["FuncName"] = "ChangeNpcSuitSuccess",
            ["NpcId"] = req?.NpcId ?? 0,
            ["SuitId"] = req?.SuitId ?? 0
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

[HouseFunc("ChangeNpcSuitByAreaId")]
public class ChangeNpcSuitByAreaId : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var req = JsonSerializer.Deserialize<NpcSuitParam>(param);
        var rsp = new JsonObject
        {
            ["FuncName"] = "ChangeNpcSuitByAreaIdRsp",
            ["NpcId"] = req?.NpcId ?? 0,
            ["SuitId"] = req?.SuitId ?? 0
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

[HouseFunc("ChangeGirlBeachSuitId")]
public class ChangeGirlBeachSuitId : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var req = JsonSerializer.Deserialize<NpcSuitParam>(param);
        var rsp = new JsonObject
        {
            ["FuncName"] = "ChangeGirlBeachSuitIdSuccess",
            ["NpcId"] = req?.NpcId ?? 0,
            ["SuitId"] = req?.SuitId ?? 0
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

internal sealed class NpcSuitParam
{
    [JsonPropertyName("NpcId")] public int NpcId { get; set; }
    [JsonPropertyName("SuitId")] public int SuitId { get; set; }
}
