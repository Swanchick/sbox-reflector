using System.Threading.Channels;
using System.Threading.Tasks;

public class Reflector : Component, Component.INetworkListener
{
	[Property]
	private GameObject playerPrefab;

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

	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
		if ( !playerPrefab.IsValid() )
			return;

		GameObject player = playerPrefab.Clone(Transform.World.WithScale( 0 ).WithRotation(Rotation.Identity), name: $"Player - {channel.DisplayName}");

	}
}
