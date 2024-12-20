public interface IKillFeed : ISceneEvent<IKillFeed>
{
	void AddKill( Player attacker, Player victim );
	void Test( string attacker, string victim );
}
