using System;


public sealed class PlayerManager : Component 
{
	public static PlayerManager instance;

	public Player LocalPlayer;

	[Sync]
	public NetList<Guid> PlayerIds { get; set; } = new();

	public List<Player> Players => PlayerIds
				.Select(id => Scene.Directory.FindByGuid(id))
				.Where(gameObject => gameObject != null)
				.Select( gameObject => gameObject.Components.Get<Player>() )
				.Where(player => player != null)
				.ToList();

	protected override void OnStart()
	{
		instance = this;
	}

	public Player FindPlayer(Guid playerId)
	{
		GameObject playerObject = Scene.Directory.FindByGuid( playerId );
		if ( !playerObject.IsValid() )
			return null;
		
		Player player = playerObject.Components.Get<Player>();
		if ( !player.IsValid() )
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

	public void AddKill(Player victim) 
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		reflector.Gamemode.OnPlayeKill( victim );
	}

	public void AddKill(Player attacker, Player victim)
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		reflector.Gamemode.OnPlayerKill( attacker, victim );
	}

	[Rpc.Broadcast]
	public void OnPlayerHit( Player attacker, Player victim )
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		reflector.Gamemode.OnPlayerHit( attacker, victim );

		if ( attacker.GameObject.Id == victim.GameObject.Id )
			return;

		victim.LastAttacker = attacker.GameObject.Id;
	}

	[Rpc.Broadcast]
	public void OnPlayerDeath( Player player )
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		reflector.Gamemode.OnPlayerDeath( player );

		Guid attackerId = player.LastAttacker;

		SendToKillFeed( attackerId, player );
	}

	private void SendToKillFeed( Guid attackerId, Player victim )
	{
		PlayerManager pm = instance;
		if (!pm.IsValid())
			return;

		victim.Stats.AddDeath();

		Player attacker = FindPlayer( attackerId );
		if ( !attacker.IsValid() )
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

		if ( !LocalPlayer.IsValid() )
			return;

		Log.Info( $"{message.AuthorName}: {message.Content}" );
		LocalPlayer.ClientHUD.Chat.AddMessage( message );
	}

	[Rpc.Broadcast]
	public void SayMessage( string content )
	{
		Message message = new Message( "Server", content );
		message.AuthorColor = new Color( 52, 168, 235 );

		if ( !LocalPlayer.IsValid() )
			return;

		Log.Info( $"{message.AuthorName}: {message.Content}" );
		LocalPlayer.ClientHUD.Chat.AddMessage( message );
	}

	[ConCmd( "add_test_kill" )]
	public static void AddTestKill()
	{
		instance.LocalPlayer.ClientHUD.KillFeed.AddKill( "Test 1", "Test 2" );
	}

	[ConCmd( "show_local_player" )]
	public static void ShowLocalPlayer()
	{
		Log.Info( $"Local player name: {instance.LocalPlayer.Name}" );
	}

	[ConCmd( "player_list" )]
	public static void ShowPlayerList()
	{
		foreach ( Player player in instance.Players )
		{
			Log.Info( $"Player: {player.Name}" );
		}
	}

	[ConCmd( "say" )]
	public static void SayCommand( string content )
	{
		if ( string.IsNullOrEmpty( content ) )
			return;

		PlayerManager pm = instance;
		if ( !pm.IsValid() )
			return;

		if ( !pm.LocalPlayer.IsValid() )
		{
			pm.SayMessage( content );
		}
		else
		{
			pm.SayMessage( pm.LocalPlayer.Name, content );
		}
	}

	[ConCmd( "show_stats" )]
	public static void ShowStats()
	{
		PlayerManager pm = instance;
		if ( !pm.IsValid() )
			return;

		if ( !pm.LocalPlayer.IsValid() )
			return;

		Log.Info( $"Kills: {pm.LocalPlayer.Stats.Kills}" );
		Log.Info( $"Deaths: {pm.LocalPlayer.Stats.Deaths}" );
		Log.Info( $"Ping: {pm.LocalPlayer.Stats.Ping}" );
	}
}
