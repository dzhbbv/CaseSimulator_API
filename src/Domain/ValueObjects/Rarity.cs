using System.Globalization;

namespace CaseSimulator.Domain.ValueObjects;

public record Rarity
{
    private readonly static string[] Arr = ["Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic", "Celestial"];
    public string Name { get; protected set; } = string.Empty;
    public string Color { get; protected set; }
    public byte Tier { get; protected set; }

    private Rarity(string name, string color, byte tier)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(color) || color.Length != 7)
            throw new ArgumentException("Rarity name and tier are required");
        if (tier < 1 || tier > 7)
            throw new ArgumentException("Rarity tier must be between 1 and 7");
        if (!Arr.Contains(name))
            throw new ArgumentException("Rarity name is incorrect");
        Name = name;
        Color = color;
        Tier = tier;
    }
    public static readonly Rarity Common     = new("Common",     "#b0c3d9", 1);
    public static readonly Rarity Uncommon   = new("Uncommon",   "#5e98d9", 2);
    public static readonly Rarity Rare       = new("Rare",       "#4b69ff", 3);
    public static readonly Rarity Epic       = new("Epic",       "#8847ff", 4);
    public static readonly Rarity Legendary  = new("Legendary",  "#d32ce6", 5);
    public static readonly Rarity Mythic     = new("Mythic",     "#eb4b4b", 6);
    public static readonly Rarity Celestial  = new("Celestial",  "#e4ae39", 7);
    
    public static Rarity FromName(string name) => name switch
    {
        "Common"    => Common,
        "Uncommon"  => Uncommon,
        "Rare"      => Rare,
        "Epic"      => Epic,
        "Legendary" => Legendary,
        "Mythic"    => Mythic,
        "Celestial" => Celestial,
        _ => throw new ArgumentException($"Unknown rarity: {name}")
    };
}
