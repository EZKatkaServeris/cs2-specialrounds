using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace SpecialRounds;


public class ConfigSpecials : BasePluginConfig
{
    [JsonPropertyName("Prefix")] public string Prefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Green}MadGames.eu{ChatColors.Default}]";
    [JsonPropertyName("mp_buytime")] public int mp_buytime { get; set; } = 15;
    [JsonPropertyName("AllowKnifeRound")] public bool AllowKnifeRound { get; set; } = true;
    [JsonPropertyName("AllowBHOPRound")] public bool AllowBHOPRound { get; set; } = true;
    [JsonPropertyName("AllowGravityRound")] public bool AllowGravityRound { get; set; } = true;
    [JsonPropertyName("AllowAWPRound")] public bool AllowAWPRound { get; set; } = true;
    [JsonPropertyName("AllowP90Round")] public bool AllowP90Round { get; set; } = true;
    [JsonPropertyName("AllowANORound")] public bool AllowANORound { get; set; } = true;
    [JsonPropertyName("AllowSlapRound")] public bool AllowSlapRound { get; set; } = true;
    [JsonPropertyName("AllowDecoyRound")] public bool AllowDecoyRound { get; set; } = true;
    [JsonPropertyName("AllowSpeedRound")] public bool AllowSpeedRound { get; set; } = true;
    [JsonPropertyName("Chance")] public int Chance { get; set; } = 100;

}
