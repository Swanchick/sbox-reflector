using System;

public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;
	public static Player LocalPlayer;


	// Todo: Save player and sync it between all clients
	[Sync]
	public NetList<Guid> Players { get; set; } = new();

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
		// Player ply = instance.Scene.;
		// ply.ClientHUD.KillFeed.AddKill(ply.Name, "Test");
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

	public void OnPlayerConnect(Player player)
	{
		// ToDo: add player on connect callback
	}
}
