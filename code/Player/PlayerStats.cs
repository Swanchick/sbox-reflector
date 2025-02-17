public sealed class PlayerStats : Component
{
	[Property]
	public Player Player { get; set; }

	public int Kills { get; set; } = 0;

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
