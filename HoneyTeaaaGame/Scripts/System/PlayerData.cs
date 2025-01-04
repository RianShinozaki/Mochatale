using Godot;
using System;
using System.Security.Cryptography.X509Certificates;


public partial class PlayerData : Node
{
	[ExportGroup("Dynamic Variables")]
	[Export] public int level = 1;
	[Export] public int exp = 0;

    [ExportGroup("Permanent Variables")]

    [Export] public PlayerLevelData[] levels = new PlayerLevelData[0];

	[ExportGroup("Member References")]
	public static PlayerData Instance;
	[Export] public Godot.Collections.Array<PackedScene> gemsEquipped;

    public override void _Ready()
    {
        base._Ready();
		Instance = this;
    }

	public static int GetLevel() {
		return Instance.GetLevelInst();
	}
	public int GetLevelInst() {
		int potentialLevel = 0;
		while(exp >= levels[potentialLevel+1].expNeeded) {
			potentialLevel++;
		}
		level = potentialLevel;
		return potentialLevel;
	}
	public static PlayerLevelData GetLevelData(int level = -1) {
		return Instance.GetLevelDataInst(level);
	}
	public PlayerLevelData GetLevelDataInst(int level) {
		if(level == -1) level = GetLevelInst();
		return levels[level];
	}
}
