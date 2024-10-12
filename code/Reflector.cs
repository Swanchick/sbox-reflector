using System;
using System.Threading.Channels;
using System.Threading.Tasks;

public class Reflector : Component, Component.INetworkListener, IReflector
{
	[Property]
	private GameObject playerPrefab;

	[Property]
	private List<GameObject> spawnPoints = new();
	private NetList<Guid> playerIds = new();


	private Dictionary<Guid, Guid> hitedPlayers = new();

	public List<GameObject> PlayerObjects
	{
		get
		{
			List<GameObject> playerObjects = new();

			foreach ( Guid playerId in playerIds )
			{
				GameObject playerObject = Scene.Directory.FindByGuid( playerId );
				if ( playerObject == null )
					continue;

				playerObjects.Add( playerObject );
			}

			return playerObjects;
		}
	}

	public GameObject GetPlayer( Guid guid )
	{
		return Scene.Directory.FindByGuid( guid );
	}

	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
		if ( !playerPrefab.IsValid() )
			return;

		Transform spawnPoint = GetRandomSpawnpoint();

		GameObject playerObject = playerPrefab.Clone( spawnPoint.WithScale( 1 ), name: $"Player - {channel.DisplayName}" );
		playerObject.NetworkSpawn( channel );

		_ = SetupPlayer( playerObject );
	}

	[Broadcast]
	public void OnPlayerHit( Player attacker, Player victim )
	{
		hitedPlayers[victim.GameObject.Id] = attacker.GameObject.Id;
	}

	[Broadcast]
	public void OnPlayerDeath( Player player )
	{
		Guid playerId = player.GameObject.Id;
		
		if ( hitedPlayers.ContainsKey( playerId ) )
		{
			GameObject attackerObject = Scene.Directory.FindByGuid( hitedPlayers[playerId] );
			Player attacker = attackerObject.Components.Get<Player>();

			Log.Info( $"{attacker.Name} killed {player.Name}" );
			
			hitedPlayers.Remove( playerId );


			return;
		}

		Log.Info( $"{player.Name}: Died" );
	}

	[Broadcast]
	public void OnPlayerGrounded( Player player )
	{
		Guid playerId = player.GameObject.Id;
		if ( hitedPlayers.ContainsKey( playerId ) )
		{
			hitedPlayers.Remove( playerId );
		}
		
		Log.Info( $"{player.Name}: has landed on the ground" );
	}

	protected override async Task OnLoad()
	{
		if ( Scene.IsEditor )
			return;

		if ( Networking.IsActive )
			return;

		LoadingScreen.Title = "Creating Lobby";
		await Task.DelayRealtimeSeconds( 0.1f );
		Networking.CreateLobby();
	}

	private async Task SetupPlayer( GameObject playerObject )
	{
		await Task.Delay( 1 );
		Player player = playerObject.GetComponent<Player>();
		//player.Spectate( true );
	}

	private Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];


		return spawn.Transform.World;
	}
}
