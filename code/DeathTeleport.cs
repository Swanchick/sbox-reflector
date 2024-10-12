using Sandbox;
using System.Threading.Tasks;

public class DeathTeleport : BaseTrigger
{
	[Property]
	private List<GameObject> spawnPoints;


	protected override void OnPlayerEnter( Player player )
	{
		player.CanUseTrigger = false;
		player.Transform.World = GetRandomSpawnpoint();
		player.CanUseTrigger = true;

		Scene.RunEvent<IReflector>( x => x.OnPlayerDeath( player ) );
		
	}

	private Transform GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];


		return spawn.Transform.World;
	}
}
