public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;

	protected override void OnAwake()
	{
		instance = this;
	}

	[ConCmd("test_command")]
	public static void TestCommand() 
	{
		Log.Info("Hello World");
	}


	public void AddKill(Player victim) 
	{

	}

	public void AddKill(Player attacker, Player victim)
	{

	}
}
