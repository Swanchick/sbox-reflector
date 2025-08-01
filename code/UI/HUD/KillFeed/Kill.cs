public class Kill
{
	public string Attacker { get; private set; }
	public string Victim { get; private set; }

	public Kill( Player attacker, Player victim )
	{
		Attacker = attacker.Name;
		Victim = victim.Name;
	}

	public Kill( string attacker, string victim )
	{
		Attacker = attacker;
		Victim = victim;
	}
}
