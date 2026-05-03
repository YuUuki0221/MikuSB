using System.Text.Json.Nodes;

namespace MikuSB.GameServer.Server.CallGS.Handlers.House;

// GameEnterMainUI (Throw) — tblockGirlList empty = no blocked girls.
[HouseFunc("GameEnterMainUI")]
public class ThrowGameEnterMainUI : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject
        {
            ["FuncName"] = "GameEnterMainUI",
            ["tblockGirlList"] = new JsonArray()
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

[HouseFunc("ThrowGameTutorialFinish")]
public class ThrowGameTutorialFinish : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ThrowGameTutorialFinish" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ThrowGameEnter — returns nSeed for level generation.
[HouseFunc("ThrowGameEnter")]
public class ThrowGameEnter : IHouseFuncHandler
{
    private static readonly Random Random = new();

    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject
        {
            ["FuncName"] = "ThrowGameEnter",
            ["nSeed"] = Random.Next(1, 1_000_000_000)
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ThrowGameSettlement — nAddExp=0 on private server.
[HouseFunc("ThrowGameSettlement")]
public class ThrowGameSettlement : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ThrowGameSettlement", ["nAddExp"] = 0 };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

[HouseFunc("ThrowGameGetLevelReward")]
public class ThrowGameGetLevelReward : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ThrowGameGetLevelReward" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

[HouseFunc("ThrowGameGetAchReward")]
public class ThrowGameGetAchReward : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ThrowGameGetAchReward" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}
