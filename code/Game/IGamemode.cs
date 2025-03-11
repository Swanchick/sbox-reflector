using Sandbox;


public interface IGamemode
{
	string Name { get; }
	string Description { get; }

	void OnGameStart();

	void OnRoundStart();
	void OnRoundEnd();

	void OnPlayerJoin( Player player );
	void OnPlayerLeave( Player player );
	void OnPlayerSpawn( Player player );
	void OnPlayerDeath( Player player );

	void OnPlayerKill( Player attacker, Player victim );
	void OnPlayerHit( Player attacker, Player victim  );
}
