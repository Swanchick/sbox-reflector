public class Item : BaseTrigger
{
	[Property]
	private GameObject diskThrower;

	protected override void OnPlayerEnter( Player player )
	{
		DiskWeapon diskWeapon = player.DiskWeapon;
		if ( diskWeapon == null )
			return;

		if ( !diskWeapon.CanAddThrower() )
			return;

		diskWeapon.AddThrower( diskThrower );
		
		GameObject.Destroy();
	}
}
