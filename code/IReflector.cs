public interface IReflector : ISceneEvent<IReflector>
{
	void OnPlayerHit( Player attacker, Player victim );
	void OnPlayerDeath( Player player );
}
