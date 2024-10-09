using System;

public class Item : BaseTrigger
{
	[Property]
	private GameObject diskThrower;

	[Sync]
	public Guid ItemStandId { get; set; }

	protected override void OnPlayerEnter( Player player )
	{
		Log.Info( "Test 1" );

		DiskWeapon diskWeapon = player.DiskWeapon;

		if ( diskWeapon == null )
			return;

		Log.Info( "Test 2" );

		if ( !diskWeapon.CanAddThrower() )
			return;

		Log.Info( "Test 3" );

		GameObject standObject = Scene.Directory.FindByGuid( ItemStandId );

		if ( standObject == null )
			return;

		Log.Info( "Test 4" );

		ItemStand stand = standObject.Components.Get<ItemStand>();
		stand.OnItemPickup();

		diskWeapon.AddThrower( diskThrower );

		Log.Info( IsProxy );

		TryToDestroy();
	}

	[Broadcast(NetPermission.Anyone)]
	private void TryToDestroy()
	{
		Log.Info( "Hello World" );

		GameObject.Destroy();
	}
}
