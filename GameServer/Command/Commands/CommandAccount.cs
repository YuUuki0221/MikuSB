using MikuSB.Database.Account;
using MikuSB.Enums.Player;
using MikuSB.Internationalization;

namespace MikuSB.GameServer.Command.Commands;

[CommandInfo("account", "Game.Command.Account.Desc", "Game.Command.Account.Usage", [], [PermEnum.Admin, PermEnum.Support])]
public class CommandAccount : ICommands
{
    [CommandMethod("create")]
    public async ValueTask Create(CommandArg arg)
    {
        if (!await arg.CheckArgCnt(2))
            return;

        var email = arg.Args[0].Trim();
        if (!int.TryParse(arg.Args[1], out var uid) || uid <= 0)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        try
        {
            var account = AccountData.CreateAccount(email, uid, "");
            await arg.SendMsg(I18NManager.Translate("Game.Command.Account.Created", account.Username, account.Uid.ToString()));
        }
        catch (InvalidOperationException ex)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Account.CreateFailed", ex.Message));
        }
        catch (ArgumentException ex)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Account.CreateFailed", ex.Message));
        }
    }
}
