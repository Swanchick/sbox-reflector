using System;

public class DeathTeleport : BaseTrigger
{
	[Property]
	private List<GameObject> spawnPoints;

	protected override void OnPlayerEnter( Player player )
	{
		if ( player.IsDead )
			return;

		GameObject randomGameObject = GetRandomSpawnpoint();
		player.Transform.ClearInterpolation();
		player.Transform.World = randomGameObject.Transform.World;
		player.Movement.PlayerController.Velocity = Vector3.Zero;
		player.Transform.ClearInterpolation();


		PlayerManager.instance.OnPlayerDeath(player);
		
		player.LastAttacker = Guid.Empty;
	}

	private GameObject GetRandomSpawnpoint()
	{
		GameObject spawn = spawnPoints[Game.Random.Next( 0, spawnPoints.Count - 1 )];

		return spawn;
	}
}
