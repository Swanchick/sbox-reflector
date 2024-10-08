public class Item : BaseTrigger
{
	[Property]
	private GameObject diskThrower;

	public ItemStand ItemStand { get; set; }

	protected override void OnPlayerEnter( Player player )
	{
		DiskWeapon diskWeapon = player.DiskWeapon;
		if ( diskWeapon == null )
			return;

		if ( !diskWeapon.CanAddThrower() )
			return;

		if ( ItemStand == null )
			return;

		ItemStand.OnItemPickup();

		diskWeapon.AddThrower( diskThrower );

		GameObject.Destroy();
	}
}
