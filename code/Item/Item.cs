using System;

public class Item : BaseTrigger
{
	[Property]
	private GameObject diskThrower;

	[Sync]
	public Guid ItemStandId { get; set; }

	protected override void OnPlayerEnter( Player player )
	{
		// ToDo:
		// Fix issue with client that tries to take power up from other client not host.

		Log.Info($"Player: {player.Name} entered the trigger to get powerup");
		

		// For some reason this diskWeapon is null, but it should be.
		DiskWeapon diskWeapon = player.DiskWeapon;

		Log.Info(diskWeapon == null);

		if ( diskWeapon == null )
			return;

		Log.Info("Hello World 1");

		if ( !diskWeapon.CanAddThrower() )
			return;
		
		Log.Info("Hello World 2");

		GameObject standObject = Scene.Directory.FindByGuid( ItemStandId );

		if ( standObject == null )
			return;

		Log.Info("Hello World 3");

		ItemStand stand = standObject.Components.Get<ItemStand>();
		stand.OnItemPickup();

		diskWeapon.AddThrower( diskThrower );

		TryToDestroy();
	}

	[Rpc.Broadcast(NetFlags.HostOnly)]
	private void TryToDestroy()
	{
		GameObject.Destroy();
	}
}
