public sealed class PlayerRow
{
	public Player Player { get; set; }

	public string Name => Player.Name;
	public string SteamId => Player.SteamId;


	public PlayerRow(Player player)
	{
		Player = player;
	}

	public int Kills => Player.Stats.Kills;

	public int Deaths => Player.Stats.Deaths;

	public int Ping => (int)Player.Stats.Ping;
}
