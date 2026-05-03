using MikuSB.Proto;
using System.Text.Json.Nodes;

namespace MikuSB.GameServer.Server.CallGS.Handlers.House;

// ArcadeGameEnterMainUI
// Returns all girl IDs (1-25) as unlocked, and syncs TeachMode + EndlessMode attrs.
[HouseFunc("ArcadeGameEnterMainUI")]
public class ArcadeGameEnterMainUI : IHouseFuncHandler
{
    private const uint ArcadeGid = 101;
    private const uint EndlessModeStateSid = 18000 + 5;
    private const uint TeachModeConditionSid = 18000 + 36 + 8;

    public async Task Handle(Connection connection, string param)
    {
        var girlList = new JsonArray();
        for (int i = 1; i <= 25; i++) girlList.Add(i);

        var rsp = new JsonObject
        {
            ["FuncName"] = "ArcadeGameEnterMainUI",
            ["tbUnlockGirlList"] = girlList
        };

        var player = connection.Player!;
        var sync = new NtfSyncPlayer();
        sync.Custom[player.ToPackedAttrKey(ArcadeGid, TeachModeConditionSid)] = 1;
        sync.Custom[player.ToShiftedAttrKey(ArcadeGid, TeachModeConditionSid)] = 1;
        const uint endlessAllUnlocked = 0x3FFFFFE;
        sync.Custom[player.ToPackedAttrKey(ArcadeGid, EndlessModeStateSid)] = endlessAllUnlocked;
        sync.Custom[player.ToShiftedAttrKey(ArcadeGid, EndlessModeStateSid)] = endlessAllUnlocked;

        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString(), sync);
    }
}

// ArcadeGameEnter — returns a random seed for level generation.
[HouseFunc("ArcadeGameEnter")]
public class ArcadeGameEnter : IHouseFuncHandler
{
    private static readonly Random Random = new();

    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject
        {
            ["FuncName"] = "ArcadeGameEnter",
            ["nSeed"] = Random.Next(1, 1_000_000_000)
        };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ArcadeGameSettlement — acknowledges round end; nAddExp=0 on private server.
[HouseFunc("ArcadeGameSettlement")]
public class ArcadeGameSettlement : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ArcadeGameSettlement", ["nAddExp"] = 0 };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ArcadeGameLogSettlement — acknowledges log upload (no client data required).
[HouseFunc("ArcadeGameLogSettlement")]
public class ArcadeGameLogSettlement : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ArcadeGameLogSettlement" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ArcadeGameGetLevelReward — UI refresh only on client side.
[HouseFunc("ArcadeGameGetLevelReward")]
public class ArcadeGameGetLevelReward : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ArcadeGameGetLevelReward" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}

// ArcadeGameGetAchReward — UI refresh only on client side.
[HouseFunc("ArcadeGameGetAchReward")]
public class ArcadeGameGetAchReward : IHouseFuncHandler
{
    public async Task Handle(Connection connection, string param)
    {
        var rsp = new JsonObject { ["FuncName"] = "ArcadeGameGetAchReward" };
        await CallGSRouter.SendScript(connection, "House_Request", rsp.ToJsonString());
    }
}
