public class Kill
{
	public Player Attacker { get; private set; }
	public Player Victim { get; private set; }

	public Kill( Player attacker, Player victim )
	{
		Attacker = attacker;
		Victim = victim;
	}
}
