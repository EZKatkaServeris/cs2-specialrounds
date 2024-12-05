using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;

namespace SpecialRounds;

[MinimumApiVersion(120)]
public static class GetUnixTime
{
    public static int GetUnixEpoch(this DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() -
                       new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (int)unixTime.TotalSeconds;
    }
}
public partial class SpecialRounds : BasePlugin, IPluginConfig<ConfigSpecials>
{
    public override string ModuleName => "SpecialRounds";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "Simple Special rounds.";
    public override string ModuleVersion => "V. 1.0.6";
    private static readonly int?[] IsVIP = new int?[65];
    public CounterStrikeSharp.API.Modules.Timers.Timer? TimerSlap;
    public CounterStrikeSharp.API.Modules.Timers.Timer? TimerDecoy;

    public ConfigSpecials Config { get; set; }
    public int Round;
    public bool EndRound;
    public bool IsRound;
    /*
    * 1 - Knife
    * 2 - BHop
    * 3 - Gravity
    * 4 - AWP
    */
    public int IsRoundNumber;
    public string NameOfRound = "";
    public bool isset = false;
    public uint[] ExclusionZone = Array.Empty<uint>();

    public void OnConfigParsed(ConfigSpecials config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        WriteColor("Special round is [*Loaded*]", ConsoleColor.Green);
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            EndRound = false;
            IsRound = false;
            NameOfRound = "";
            IsRoundNumber = 0;
            Round = 0;
        });

        RegisterListener<Listeners.OnTick>(() =>
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;

                var client = new CCSPlayerController(ent);
                if (client == null || !client.IsValid)
                    continue;
                if (IsRound)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='green'>Special Rounds</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>Now playing</font> <font class='fontSize-m' color='green'>[{NameOfRound}]</font>"
                    );
                }
                OnTick(client);
            }
        });
    }
    public static SpecialRounds It;
    public SpecialRounds()
    {
        It = this;
    }
    public static void OnTick(CCSPlayerController controller)
    {
        if (!controller.PawnIsAlive)
            return;
        var pawn = controller.Pawn.Value;
        var flags = (PlayerFlags)pawn.Flags;
        var client = controller.Index;
        var buttons = controller.Buttons;


        if (It.IsRoundNumber != 6)
            return;
        if (buttons == PlayerButtons.Attack2)
            return;
        if (buttons == PlayerButtons.Zoom)
            return;
    }

    [ConsoleCommand("css_startround", "Start specific round")]
    public void startround(CCSPlayerController? player, CommandInfo info)
    {
        if (AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            int round_id = Convert.ToInt32(info.ArgByIndex(1));
            if (round_id == null)
            {
                return;
            }
            EndRound = false;
            IsRound = true;
            IsRoundNumber = round_id;
            player.PrintToChat("YOU STARTED A ROUND!");
        }
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (EndRound)
        {
            WriteColor($"SpecialRound - [*SUCCESS*] I stopped the special round.", ConsoleColor.Green);
            if (IsRoundNumber == 1)
            {
                change_cvar("sv_buy_status_override", "-1");
                change_cvar("mp_buytime", $"{Config.mp_buytime}");
            }
            if (IsRoundNumber == 2)
            {
                change_cvar("sv_autobunnyhopping", "false");
                change_cvar("sv_enablebunnyhopping", "false");
            }
            if (IsRoundNumber == 3)
            {
                change_cvar("sv_gravity", "800");
            }
            if (IsRoundNumber == 4)
            {
                change_cvar("sv_buy_status_override", "-1");
                change_cvar("mp_buytime", $"{Config.mp_buytime}");
            }
            if (IsRoundNumber == 5)
            {
                change_cvar("sv_buy_status_override", "-1");
                change_cvar("mp_buytime", $"{Config.mp_buytime}");
            }
            if (IsRoundNumber == 6)
            {
                change_cvar("sv_buy_status_override", "-1");
                change_cvar("mp_buytime", $"{Config.mp_buytime}");
            }
            if (IsRoundNumber == 7)
            {
                TimerSlap?.Kill();
                Array.Clear(ExclusionZone);
            }
            if (IsRoundNumber == 8)
            {
                change_cvar("sv_buy_status_override", "-1");
                change_cvar("mp_buytime", $"{Config.mp_buytime}");
                TimerDecoy?.Kill();
            }
            IsRound = false;
            EndRound = false;
            isset = false;
            IsRoundNumber = 0;
            NameOfRound = "";
        }

        if (IsRound)
        {
            WriteColor($"SpecialRound - [*WARNING*] Cannot start new special round when it's already in progress.", ConsoleColor.Yellow);
            return HookResult.Continue;
        }
        if (Round < 0)
        {
            WriteColor("SpecialRound - [*WARNING*] Cannot start new special round, it's round < 5.", ConsoleColor.Yellow);
            return HookResult.Continue;
        }

        Random rnd = new();
        int random = rnd.Next(0, Config.Chance);
        if (random >= 0 && random < 3)
        {
            if (Config.AllowKnifeRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 1;
                NameOfRound = "Knife only";
            }
        }
        if (random == 3)
        {
            if (Config.AllowBHOPRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 2;
                NameOfRound = "Auto BHopping";
            }
        }
        if (random >= 4 && random < 6)
        {
            if (Config.AllowGravityRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 3;
                NameOfRound = "Gravity round";
            }
        }
        if (random == 6)
        {
            if (Config.AllowAWPRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 4;
                NameOfRound = "Only AWP";
            }
        }
        if (random == 7)
        {
            if (Config.AllowP90Round)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 5;
                NameOfRound = "Only Deagle";
            }
        }
        if (random >= 8 && random < 11)
        {
            if (Config.AllowANORound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 6;
                NameOfRound = "Only AWP + NOSCOPE";
            }
        }
        if (random >= 11 && random < 13)
        {
            if (Config.AllowSlapRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 7;
                NameOfRound = "Slapping round";
            }
        }
        if (random >= 13 && random < 16)
        {
            if (Config.AllowDecoyRound)
            {
                IsRound = true;
                EndRound = true;
                IsRoundNumber = 8;
                NameOfRound = "Decoy round";
            }
        }
        if (IsRound == true)
        {
            WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound} Number is:{random}.", ConsoleColor.Green);
        }
        //Server.PrintToConsole($" Settings : {NameOfRound} / IsRound {IsRound} / IsRoundNumber {IsRoundNumber} / Random number {random}");

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundPostStart(EventRoundPoststart @event, GameEventInfo info)
    {
        if (IsRound && IsRoundNumber > 0 && IsWeaponRound(IsRoundNumber))
        {
            if (IsRoundNumber == 1 && Config.AllowKnifeRound)
                change_cvar("sv_buy_status_override", "3");
            if (IsRoundNumber == 4 && Config.AllowAWPRound)
                change_cvar("sv_buy_status_override", "3");
            if (IsRoundNumber == 5 && Config.AllowP90Round)
                change_cvar("sv_buy_status_override", "3");
            if (IsRoundNumber == 6 && Config.AllowANORound)
                change_cvar("sv_buy_status_override", "3");
            if (IsRoundNumber == 8 && Config.AllowDecoyRound)
                change_cvar("sv_buy_status_override", "3");
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnItemPurchase(EventItemPurchase @event, GameEventInfo info)
    {
        if (IsRound && IsRoundNumber > 0 && IsWeaponRound(IsRoundNumber))
        {
            string weaponName = @event.Weapon;
            CCSPlayerController user = @event.Userid;
            if (user.IsValid && user.PawnIsAlive && user.PlayerPawn.Value is not null)
            {
                foreach (var weapon in user.PlayerPawn.Value.WeaponServices!.MyWeapons)
                {
                    if (weapon is { IsValid: true, Value.IsValid: true } && weapon.Value.DesignerName.Equals(weaponName))
                    {
                        weapon.Value.Remove();
                    }
                }
            }
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (GameRules().WarmupPeriod)
        {
            IsRound = false;
            EndRound = false;
            isset = false;
            IsRoundNumber = 0;
            NameOfRound = "";
        }
        foreach (var l_player in Utilities.GetPlayers())
        {
            CCSPlayerController player = l_player;
            var client = player.Index;
            if (IsRoundNumber == 1)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowKnifeRound)
                {
                    if (!is_alive(player) || player.PlayerPawn.Value is null)
                        return HookResult.Continue;
                    foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
                    {
                        if (weapon is { IsValid: true, Value.IsValid: true })
                        {
                            if (RemoveWeapon(weapon.Value.DesignerName))
                            {
                                continue;
                            }
                            change_cvar("mp_buytime", "0");
                            weapon.Value.Remove();
                        }
                    }
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 2)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowBHOPRound)
                {
                    change_cvar("sv_autobunnyhopping", "true");
                    change_cvar("sv_enablebunnyhopping", "true");
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 3)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowGravityRound)
                {
                    change_cvar("sv_gravity", "400");
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 4)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowAWPRound)
                {
                    if (!is_alive(player) || player.PlayerPawn.Value is null)
                        return HookResult.Continue;
                    foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
                    {
                        if (weapon is { IsValid: true, Value.IsValid: true })
                        {
                            if (RemoveWeapon(weapon.Value.DesignerName))
                            {
                                continue;
                            }
                            change_cvar("mp_buytime", "0");
                            weapon.Value.Remove();
                        }
                    }
                    player.GiveNamedItem("weapon_awp");
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 5)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowP90Round)
                {
                    if (!is_alive(player) || player.PlayerPawn.Value is null)
                    {
                        return HookResult.Continue;
                    }

                    foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
                    {
                        if (weapon is { IsValid: true, Value.IsValid: true })
                        {
                            if (RemoveWeapon(weapon.Value.DesignerName))
                            {
                                continue;
                            }
                            weapon.Value.Remove();
                        }
                    }

                    change_cvar("mp_buytime", "0");
                    player.GiveNamedItem("weapon_deagle");
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 6)
            {
                WriteColor($"SpecialRound - [*ROUND START*] Starting special round {NameOfRound}.", ConsoleColor.Green);

                if (IsRound || Config.AllowANORound)
                {
                    if (!is_alive(player) || player.PlayerPawn.Value is null)
                        return HookResult.Continue;
                    foreach (var weapon in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
                    {
                        if (weapon is { IsValid: true, Value.IsValid: true })
                        {
                            if (RemoveWeapon(weapon.Value.DesignerName))
                            {
                                continue;
                            }
                            weapon.Value.Remove();
                        }
                    }
                    change_cvar("mp_buytime", "0");
                    player.GiveNamedItem("weapon_awp");
                    if (!EndRound)
                    {
                        EndRound = true;
                    }
                }
            }
            if (IsRoundNumber == 7)
            {
                if (IsRound || Config.AllowSlapRound)
                {
                    Random rnd = new Random();
                    int random = rnd.Next(3, 10);
                    float random_time = random;
                    TimerSlap = AddTimer(random + 0.1f, () => { goup(player, ExclusionZone); }, TimerFlags.REPEAT);
                }
            }
            if (IsRoundNumber == 8)
            {
                if (IsRound || Config.AllowDecoyRound)
                {
                    foreach (var weapon in player.PlayerPawn.Value!.WeaponServices!.MyWeapons)
                    {
                        if (weapon is { IsValid: true, Value.IsValid: true })
                        {
                            if (RemoveWeapon(weapon.Value.DesignerName))
                            {
                                continue;
                            }
                            weapon.Value.Remove();
                        }
                    }
                    change_cvar("mp_buytime", "0");
                    player.PlayerPawn.Value!.Health = 1;
                    player.GiveNamedItem("weapon_decoy");
                    TimerDecoy = AddTimer(2.0f, () => { DecoyCheck(player); }, TimerFlags.REPEAT);
                    Server.PrintToConsole($"{player.PlayerName}");
                }
            }
        }
        isset = false;
        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        CCSPlayerController attacker = @event.Attacker;

        if (player.Connected != PlayerConnectedState.PlayerConnected || !player.PlayerPawn.IsValid || !@event.Userid.IsValid)
            return HookResult.Continue;
        if (IsRoundNumber == 8)
        {
            if (@event.Weapon != "decoy")
            {
                player.PlayerPawn.Value!.Health = 1;
                player.PrintToChat($" {Config.Prefix} You canno't hit player with other GUN!");
            }
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnWeaponZoom(EventWeaponZoom @event, GameEventInfo info)
    {
        if (IsRoundNumber != 6) { return HookResult.Continue; }
        var player = @event.Userid;
        var weaponservices = player.PlayerPawn.Value.WeaponServices!;
        var currentWeapon = weaponservices.ActiveWeapon.Value.DesignerName;

        weaponservices.ActiveWeapon.Value.Remove();
        player.GiveNamedItem(currentWeapon);

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnBombsiteEnter(EventEnterBombzone @event, GameEventInfo info)
    {
        if (@event.Userid.PlayerPawn.Value?.Index is not null)
        {
            var player = @event.Userid.PlayerPawn.Value;
            ExclusionZone = ExclusionZone.Append(player.Index).ToArray();
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnBombsiteLeave(EventExitBombzone @event, GameEventInfo info)
    {
        if (@event.Userid.PlayerPawn.Value?.Index is not null)
        {
            var player = @event.Userid.PlayerPawn.Value;
            ExclusionZone = ExclusionZone.Where(id => id != player.Index).ToArray();
        }
        return HookResult.Continue;
    }
}
