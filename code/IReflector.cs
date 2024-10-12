public interface IReflector : ISceneEvent<IReflector>
{
	void OnPlayerGrounded( Player player );
	void OnPlayerHit( Player attacker, Player victim );
	void OnPlayerDeath( Player player );
}
