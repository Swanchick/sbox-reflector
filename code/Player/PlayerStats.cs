public sealed class PlayerStats : Component
{
	[Property]
	public Player Player { get; set; }

	[Sync]
	public int Kills { get; set; } = 0;

	[Sync]
	public int Deaths { get; set; } = 0;

	public float Ping => Player.Network.Owner.Ping;

	public void AddKill()
	{
		Kills++;
	}
	
	public void AddDeath()
	{
		Deaths++;
	}
}
