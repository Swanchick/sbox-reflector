using System;

public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;

	public Player LocalPlayer;

	[Sync]
	public NetList<Guid> PlayerIds { get; set; } = new();

	public List<Player> Players 
	{
		get
		{
			return PlayerIds
				.Select(id => Scene.Directory.FindByGuid(id).Components.Get<Player>())
				.ToList();
		}
	}

	protected override void OnStart()
	{
		instance = this;
	}

	[Rpc.Broadcast]
	public void AddPlayer(Player player) 
	{
		if (!player.IsProxy) 
			LocalPlayer = player;

		Guid playerId = player.GameObject.Id;
		PlayerIds.Add(playerId);
	}

	[ConCmd("test_command")]
	public static void TestCommand() 
	{
		Log.Info("Hello World");
	}

	[ConCmd("add_test_kill")]
	public static void AddTestKill()
	{
		instance.LocalPlayer.ClientHUD.KillFeed.AddKill("Test 1", "Test 2");
	}

	[ConCmd("show_local_player")]
	public static void ShowLocalPlayer()
	{
		Log.Info($"Local player name: {instance.LocalPlayer.Name}");
	}

	[ConCmd("player_list")]
	public static void ShowPlayerList()
	{		
		foreach (Player player in instance.Players)
		{
			Log.Info($"Player: {player.Name}");
		}
	}

	public void AddKill(Player victim) 
	{
		if (victim == null || LocalPlayer == null)
			return;

		LocalPlayer.ClientHUD.KillFeed.AddKill("", victim.Name);
	}

	public void AddKill(Player attacker, Player victim)
	{
		if (attacker == null || victim == null || LocalPlayer == null)
			return;

		LocalPlayer.ClientHUD.KillFeed.AddKill(attacker.Name, victim.Name);
	}
}
