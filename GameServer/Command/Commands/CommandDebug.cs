using MikuSB.Configuration;
using MikuSB.Enums.Player;
using MikuSB.Util;

namespace MikuSB.GameServer.Command.Commands;

[CommandInfo("debug", "Debug packet output", "/debug [on|off|simple|detail|file]", ["dbg"], [PermEnum.Admin, PermEnum.Support])]
public class CommandDebug : ICommands
{
    private static readonly Logger Logger = new("CommandManager");

    [CommandDefault]
    public async ValueTask ToggleDebug(CommandArg arg)
    {
        var option = arg.Args.FirstOrDefault()?.ToLowerInvariant() ?? "on";
        var serverOption = ConfigManager.Config.ServerOption;
        var message = option switch
        {
            "on" => EnableDebug(serverOption),
            "off" => DisableDebug(serverOption),
            "simple" => EnableSimpleDebug(serverOption),
            "detail" => EnableDebug(serverOption),
            "file" => ToggleDebugFile(serverOption),
            _ => "Usage: /debug [on|off|simple|detail|file]"
        };

        Logger.Info(message);
        await arg.SendMsg(message);
    }

    private static string EnableDebug(ServerOption serverOption)
    {
        serverOption.EnableDebug = true;
        serverOption.DebugMessage = true;
        serverOption.DebugDetailMessage = true;
        return "Debug packet output enabled.";
    }

    private static string DisableDebug(ServerOption serverOption)
    {
        serverOption.EnableDebug = false;
        return "Debug packet output disabled.";
    }

    private static string EnableSimpleDebug(ServerOption serverOption)
    {
        serverOption.EnableDebug = true;
        serverOption.DebugMessage = true;
        serverOption.DebugDetailMessage = false;
        return "Simple debug packet output enabled.";
    }

    private static string ToggleDebugFile(ServerOption serverOption)
    {
        serverOption.SavePersonalDebugFile = !serverOption.SavePersonalDebugFile;
        return serverOption.SavePersonalDebugFile
            ? "Personal debug file output enabled."
            : "Personal debug file output disabled.";
    }
}
