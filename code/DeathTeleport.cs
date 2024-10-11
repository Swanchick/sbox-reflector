using Sandbox;
using System.Threading.Tasks;

public class DeathTeleport : BaseTrigger
{
	[Property]
	private List<GameObject> spawnPoints;

	private Reflector reflector;

	protected override void OnStart()
	{
		GameObject reflectorObject = Scene.Directory.FindByName( "ReflectorManager" ).FirstOrDefault();
		if ( reflector == null )
			return;

		reflector = reflectorObject.Components.Get<Reflector>();
	}

	protected override void OnPlayerEnter( Player player )
	{
		_ = Teleport( player );
	}

	private async Task Teleport( Player player )
	{
		player.CanUseTrigger = false;
		player.Transform.World = GetRandomSpawnpoint();
		player.playerController.Velocity = Vector3.Zero;

		await Task.Delay( 100 );

		player.playerController.Velocity = Vector3.Zero;
		player.CanUseTrigger = true;
		reflector.OnPlayerDeath( player.GameObject, player );
	}

	private Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];


		return spawn.Transform.World;
	}
}
