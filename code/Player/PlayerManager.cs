using System;

public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;

	// ToDo: Make local player :)
	public static Player LocalPlayer;

	// Todo: Save player and sync it between all clients
	[Sync]
	public NetList<Guid> PlayerIds { get; set; } = new();


	protected override void OnStart()
	{
		instance = this;
	}

	public void AddPlayer(Player player) 
	{

	}

	[ConCmd("test_command")]
	public static void TestCommand() 
	{
		Log.Info("Hello World");
	}

	[ConCmd("add_test_kill")]
	public static void AddTestKill()
	{

	}

	[Rpc.Broadcast]
	public void AddKill(Player victim) 
	{
		if (IsProxy)
			return;

		// ToDo: Add suicide
	}

	public void AddKill(Player attacker, Player victim)
	{
		// ToDo: well understandable
	}


}
