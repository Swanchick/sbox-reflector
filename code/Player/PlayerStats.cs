public sealed class PlayerStats : Component
{
	[Property]
	public Player Player { get; set; }

	[Sync]
	public int Kills { get; private set; } = 0;

	[Sync]
	public int Deaths { get; private set; } = 0;

	public float Ping => Player.Network.Owner.Ping;

	public void AddKill()
	{
		Kills++;
	}
	
	public void RemoveKill()
	{
		Kills--;
	}

	public void AddDeath()
	{
		Deaths++;
	}
}
