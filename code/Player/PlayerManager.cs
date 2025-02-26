using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;


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

	public Player FindPlayer(Guid playerId)
	{
		GameObject playerObject = Scene.Directory.FindByGuid( playerId );
		if ( playerObject == null )
			return null;
		
		Player player = playerObject.Components.Get<Player>();
		if ( player == null )
			return null;


		return player;
	}


	[Rpc.Broadcast]
	public void AddPlayer(Player player) 
	{
		if (!player.IsProxy) 
			LocalPlayer = player;

		Guid playerId = player.GameObject.Id;
		PlayerIds.Add(playerId);
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


	[ConCmd("say")]
	public static void SayCommand(string content)
	{
		if ( string.IsNullOrEmpty( content ) )
			return;

		PlayerManager pm = instance;
		if ( pm == null )
			return;

		if (pm.LocalPlayer == null )
		{
			pm.SayMessage( content );
		} 
		else
		{
			pm.SayMessage( pm.LocalPlayer.Name, content );
		}
	}

	[ConCmd("show_stats")]
	public static void ShowStats()
	{
		PlayerManager pm = instance;
		if ( pm == null )
			return;

		if ( pm.LocalPlayer == null )
			return;

		Log.Info( $"Kills: {pm.LocalPlayer.Stats.Kills}" );
		Log.Info( $"Deaths: {pm.LocalPlayer.Stats.Deaths}" );
		Log.Info( $"Ping: {pm.LocalPlayer.Stats.Ping}" );
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

	[Rpc.Broadcast]
	public void OnPlayerHit( Player attacker, Player victim )
	{
		if ( attacker.GameObject.Id == victim.GameObject.Id )
			return;

		victim.LastAttacker = attacker.GameObject.Id;
	}

	[Rpc.Broadcast]
	public void OnPlayerDeath( Player player )
	{
		Guid attackerId = player.LastAttacker;

		SendToKillFeed( attackerId, player );
	}

	private void SendToKillFeed( Guid attackerId, Player victim )
	{
		PlayerManager pm = instance;
		if (pm == null)
			return;

		victim.Stats.AddDeath();

		Player attacker = FindPlayer( attackerId );
		if ( attacker == null )
		{
			pm.AddKill( victim );
			victim.Stats.RemoveKill();
		} 
		else
		{
			pm.AddKill( attacker, victim );
			attacker.Stats.AddKill();
		}

		LocalPlayer.ClientHUD.Scoreboard.StateHasChanged();
	}

	[Rpc.Broadcast]
	public void SayMessage(string authorName, string content)
	{
		Message message = new Message( authorName, content );

		if ( LocalPlayer == null )
			return;

		Log.Info( $"{message.AuthorName}: {message.Content}" );
		LocalPlayer.ClientHUD.Chat.AddMessage( message );
	}

	[Rpc.Broadcast]
	public void SayMessage( string content )
	{
		Message message = new Message( "Server", content );
		message.AuthorColor = new Color( 52, 168, 235 );

		if ( LocalPlayer == null )
			return;

		Log.Info( $"{message.AuthorName}: {message.Content}" );
		LocalPlayer.ClientHUD.Chat.AddMessage( message );
	}
}
