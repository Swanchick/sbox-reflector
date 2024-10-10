using System;
using System.Threading.Channels;
using System.Threading.Tasks;

public class Reflector : Component, Component.INetworkListener
{
	[Property]
	private GameObject playerPrefab;

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

		GameObject playerObject = playerPrefab.Clone( Transform.World.WithScale( 1 ).WithRotation( Rotation.Identity ), name: $"Player - {channel.DisplayName}" );
		playerObject.NetworkSpawn( channel );

		_ = SetupPlayer( playerObject );
	}

	public void OnPlayerHit(GameObject playerObject, Player player)
	{

	}

	public void OnPlayerDeath(GameObject playerObject, Player player)
	{ 

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

	private async Task SetupPlayer(GameObject playerObject)
	{
		await Task.Delay( 1 );
		Player player = playerObject.GetComponent<Player>();
		player.Reflector = this;
		//player.Spectate( true );
	}
}
