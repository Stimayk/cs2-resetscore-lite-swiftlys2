using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace ResetScoreLite;

[PluginMetadata(
    Id = "ResetScoreLite",
    Version = "1.0.0",
    Name = "ResetScore (Lite)",
    Author = "E!N",
    Website = "https://nova-hosting.ru/?ref=ein"
)]
public class ResetScoreLite(ISwiftlyCore core) : BasePlugin(core)
{
    private const string CommandName = "rs";

    public override void Load(bool hotReload)
    {
        Core.Command.RegisterCommand(CommandName, OnResetScoreCommand);
    }

    public override void Unload()
    {
        Core.Command.UnregisterCommand(CommandName);
    }

    private void OnResetScoreCommand(ICommandContext command)
    {
        if (command.Sender is not { } player)
            return;

        var controller = player.Controller;
        var matchStats = controller.ActionTrackingServices?.MatchStats;

        if (IsScoreAlreadyReset(controller, matchStats))
        {
            player.SendMessage(MessageType.Chat, $"{Core.Localizer["prefix"]}{Core.Localizer["already"]}");
            return;
        }

        ResetPlayerStats(controller, matchStats);

        controller.ActionTrackingServicesUpdated();

        player.SendMessage(MessageType.Chat, $"{Core.Localizer["prefix"]}{Core.Localizer["success"]}");

        Core.PlayerManager.SendMessage(MessageType.Chat,
            $"{Core.Localizer["prefix"]}{Core.Localizer["reset", player.Controller.PlayerName]}");
    }

    private static bool IsScoreAlreadyReset(CCSPlayerController controller, CSMatchStats_t? stats)
    {
        if (stats is null)
            return controller is { Score: 0, MVPs: 0 };

        return controller is { Score: 0, MVPs: 0 } &&
               stats is
               {
                   Kills: 0, Deaths: 0, Assists: 0, Damage: 0,
                   EquipmentValue: 0, MoneySaved: 0, KillReward: 0, LiveTime: 0,
                   HeadShotKills: 0, Objective: 0, CashEarned: 0,
                   UtilityDamage: 0, EnemiesFlashed: 0
               };
    }

    private static void ResetPlayerStats(CCSPlayerController controller, CSMatchStats_t? stats)
    {
        controller.Score = 0;
        controller.MVPs = 0;

        if (stats is null) return;

        stats.Kills = 0;
        stats.Deaths = 0;
        stats.Assists = 0;
        stats.Damage = 0;
        stats.EquipmentValue = 0;
        stats.MoneySaved = 0;
        stats.KillReward = 0;
        stats.LiveTime = 0;
        stats.HeadShotKills = 0;
        stats.Objective = 0;
        stats.CashEarned = 0;
        stats.UtilityDamage = 0;
        stats.EnemiesFlashed = 0;
    }
}