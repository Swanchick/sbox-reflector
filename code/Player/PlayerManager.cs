public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;

	public List<Player> Players { get; set; } = new();

	protected override void OnStart()
	{
		instance = this;
	}

	[ConCmd("test_command")]
	public static void TestCommand() 
	{
		Log.Info("Hello World");
	}

	[ConCmd("add_test_kill")]
	public static void AddTestKill()
	{
		Player ply = instance.Players.First();
		ply.ClientHUD.KillFeed.AddKill(ply.Name, "Test");
	}

	public void AddKill(Player victim) 
	{
		
	}

	public void AddKill(Player attacker, Player victim)
	{

	}
}
