using System;

public class Item : BaseTrigger
{
	[Property]
	private GameObject diskThrower;

	[Sync]
	public Guid ItemStandId { get; set; }

	protected override void OnPlayerEnter( Player player )
	{
		DiskWeapon diskWeapon = player.DiskWeapon;

		if ( diskWeapon == null )
			return;

		if ( !diskWeapon.CanAddThrower() )
			return;

		GameObject standObject = Scene.Directory.FindByGuid( ItemStandId );

		if ( standObject == null )
			return;

		ItemStand stand = standObject.Components.Get<ItemStand>();
		stand.OnItemPickup();

		diskWeapon.AddThrower( diskThrower );

		TryToDestroy();
	}

	[Broadcast(NetPermission.Anyone)]
	private void TryToDestroy()
	{
		GameObject.Destroy();
	}
}
