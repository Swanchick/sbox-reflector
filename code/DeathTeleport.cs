public class DeathTeleport : BaseTrigger
{
	[Property]
	private List<GameObject> spawnPoints;

	protected override void OnPlayerEnter( Player player )
	{
		if ( !player.Alive )
			return;

		GameObject randomGameObject = GetRandomSpawnpoint();
		player.Jump( Vector3.Zero, 0 );
		player.Transform.World = randomGameObject.Transform.World;
		player.Transform.ClearInterpolation();

		Log.Info( $"Spawned on - {player.Transform.World.Position}" );
		Scene.RunEvent<IReflector>( x => x.OnPlayerDeath( player ) );
	}

	private GameObject GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];

		return spawn;
	}
}
