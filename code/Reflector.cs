using System.Threading.Tasks;


public class Reflector : Component, Component.INetworkListener
{
	[Property]
	private GameObject playerPrefab;

	[Property]
	private List<GameObject> spawnPoints = new();

	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
		if ( !playerPrefab.IsValid() )
			return;

		Transform spawnPoint = GetRandomSpawnpoint();

		GameObject playerObject = playerPrefab.Clone( spawnPoint.WithScale( 1 ), name: $"Player - {channel.DisplayName}" );
		playerObject.NetworkSpawn( channel );

		_ = SetupPlayer( playerObject );

		PlayerManager pm = PlayerManager.instance;
		if ( pm == null )
			return;

		pm.SayMessage( $"{channel.DisplayName} has joined the game!" );
	}

	public void OnDisconnected( Connection connection )
	{
		PlayerManager pm = PlayerManager.instance;
		if ( pm == null )
			return;

		foreach ( Player player in pm.Players )
		{
			if ( player.Connection == connection )
			{
				pm.SayMessage( $"{player.Name} has left the game!" );

				pm.PlayerIds.Remove( player.GameObject.Id );

				break;
			}
		}
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

		PlayerManager.instance.AddPlayer(player);
	}

	private Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];

		return spawn.Transform.World;
	}
}
