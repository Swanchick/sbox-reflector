using System.Threading.Tasks;


public class Reflector : Component, Component.INetworkListener
{
	[Property]
	public BaseGamemode Gamemode { get; set; }

	[Property]
	private GameObject playerPrefab;

	[Property]
	private List<GameObject> spawnPoints = new();

	public static Reflector instance;

	protected override void OnStart()
	{
		instance = this;

		Gamemode.OnGameStart();
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
		
		Gamemode.OnPlayerJoin(playerObject.Components.Get<Player>());
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
				Gamemode.OnPlayerLeave( player );
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

	public Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];

		return spawn.Transform.World;
	}
}
