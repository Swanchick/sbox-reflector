using System;
using System.Threading.Tasks;

public class Reflector : Component, Component.INetworkListener, IReflector
{
	[Property]
	private GameObject playerPrefab;

	[Property]
	private List<GameObject> spawnPoints = new();
	private NetList<Guid> playerIds = new();

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

	public void OnPlayerHit( Player attacker, Player victim )
	{
		if ( attacker.GameObject.Id == victim.GameObject.Id )
			return;

		Log.Info( $"Attacker: {attacker.GameObject.Id} =========== Victim: {victim.GameObject.Id}" );

		Log.Info( "Message for everyone" );

		victim.LastAttacker = attacker.GameObject.Id;
	}

	private void SendKillFeed( Guid attackerId, Player victim )
	{
		PlayerManager pm = PlayerManager.instance;
		if (pm == null)
			return;

		if (attackerId == Guid.Empty)
		{
			pm.AddKill(victim);
			
			return;
		}

		GameObject attackerObject = Scene.Directory.FindByGuid( attackerId );
		if ( attackerObject == null )
			return;

		Player attacker = attackerObject.Components.Get<Player>();
		if ( attacker == null )
			return;
		
		

		// ToDo: Add killfeed player attacker and victim
	}

	[Rpc.Broadcast]
	public void OnPlayerDeath( Player player )
	{
		Guid attackerId = player.LastAttacker;

		SendKillFeed( attackerId, player );
	}


	protected override async Task OnLoad()
	{
		if ( Scene.IsEditor )
			return;

		if ( Networking.IsActive )
			return;

		LoadingScreen.Title = "Creating Lobby";
		await Task.DelayRealtimeSeconds( 0.1f );
		Networking.CreateLobby( new() );
	}

	private async Task SetupPlayer( GameObject playerObject )
	{
		await Task.Delay( 1 );
		Player player = playerObject.GetComponent<Player>();

		PlayerManager.instance.OnPlayerConnect(player);
	}

	private Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];

		return spawn.Transform.World;
	}
}
