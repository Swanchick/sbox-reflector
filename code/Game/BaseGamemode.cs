public class BaseGamemode : Component, IGamemode
{
	[Property]
	private string name = "Base Gamemode";

	[Property]
	private string description = "Base Gamemode Description";

	public string Name => name;
	public string Description => description;

	public virtual void OnGameStart()
	{

	}
	
	public virtual void OnRoundStart()
	{

	}
	
	public virtual void OnRoundEnd()
	{

	}
	
	public virtual void OnPlayerJoin( Player player )
	{

	}

	public virtual void OnPlayerLeave( Player player )
	{

	}

	public virtual void OnPlayerSpawn( Player player )
	{

	}

	public virtual void OnPlayerDeath( Player player )
	{

	}

	public virtual void OnPlayerKill( Player attacker, Player victim )
	{

	}

	public virtual void OnPlayerHit( Player attacker, Player victim )
	{

	}
}
