using System;

public class DeathTeleport : BaseTrigger
{
	[Property]
	private List<GameObject> spawnPoints;

	protected override void OnPlayerEnter( Player player )
	{
		if ( !player.IsAlive )
			return;

		PlayerManager.instance.OnPlayerDeath( player );
	}
}
