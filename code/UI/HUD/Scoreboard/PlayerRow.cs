public sealed class PlayerRow
{
	public Player Player { get; set; }

	public string Name => Player.Name;
	public string SteamId => Player.SteamId;


	public PlayerRow(Player player)
	{
		Player = player;
	}
}
