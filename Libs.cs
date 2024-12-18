using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace SpecialRounds
{
    public partial class SpecialRounds
    {
        internal static CCSGameRules GameRules()
        {
            return Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
        }

        static void WriteColor(string message, ConsoleColor color)
        {
            var pieces = Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        static public int get_hp(CCSPlayerController? player)
        {
            if (player == null || !player.PawnIsAlive)
            {
                return 100;
            }
            return player.PlayerPawn.Value.Health;
        }

        private bool CheckIsHaveWeapon(string weapon_name, CCSPlayerController? pc)
        {
            if (pc == null || !pc.IsValid)
                return false;

            var pawn = pc.PlayerPawn.Value.WeaponServices!;
            foreach (var weapon in pawn.MyWeapons)
            {
                if (weapon is { IsValid: true, Value.IsValid: true })
                {
                    if (weapon.Value.DesignerName.Contains($"{weapon_name}"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void DecoyCheck(CCSPlayerController? player)
        {
            if (player is null || !player.PawnIsAlive || !player.IsValid)
                return;

            var client = player.Index;
            var pawn = player.PlayerPawn;
            if (IsRoundNumber == 8)
            {
                if (IsRound == true)
                {
                    if (CheckIsHaveWeapon("weapon_decoy", player) == false)
                    {
                        player.GiveNamedItem("weapon_decoy");
                    }
                }
                else
                {
                    TimerDecoy?.Kill();
                }
            }
            else
            {
                TimerDecoy?.Kill();
            }
        }
        public void goup(CCSPlayerController? player, uint[] excluded)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value is null)
            {
                //WriteColor($"Special Rounds - [*goup*] is not valid or is disconnected.", ConsoleColor.Red);
                return;
            }
            if (!player.PawnIsAlive)
            {
                WriteColor($"Special Rounds - [*{player.PlayerName}*] is dead.", ConsoleColor.Red);
                return;
            }
            if (IsRoundNumber == 7)
            {
                if (IsRound == true && !ExclusionZone.Contains(player.PlayerPawn.Value.Index))
                {
                    var pawn = player.Pawn.Value;
                    var random = new Random();
                    var vel = new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z);

                    vel.X += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
                    vel.Y += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
                    vel.Z += random.Next(200) + 100;

                    pawn.Teleport(pawn.AbsOrigin!, pawn.AbsRotation!, vel);
                }
                else
                {
                    TimerSlap?.Kill();
                }
            }
            else
            {
                TimerSlap?.Kill();
            }
        }
        static public bool is_alive(CCSPlayerController? player)
        {
            if (player is null || !player.PawnIsAlive)
            {
                return false;
            }
            return true;
        }

        static public bool change_cvar(string cvar, string value)
        {
            var find_cvar = ConVar.Find($"{cvar}");
            if (find_cvar == null)
            {
                WriteColor($"SpecialRound - [*ERROR*] Cannot set {cvar} to {value}.", ConsoleColor.Red);
                return false;
            }
            Server.ExecuteCommand($"{cvar} {value}");
            return true;
        }

        static public bool RemoveWeapon(string name)
        {
            string[] WeaponsToKeep = { "weapon_bayonet", "weapon_knife", "weapon_c4" };
            return WeaponsToKeep.Contains(name);
        }

        static public bool IsWeaponRound(int RoundNumber)
        {
            List<bool> NoBuyRounds = new() { false, true, false, false, true, true, true, false, true };
            return NoBuyRounds[RoundNumber];
        }
    }
}
