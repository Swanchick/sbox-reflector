public abstract class BaseDiskThrower : Component, IWeapon
{
	[Property]
	private BaseDisk disk;

	[Property]
	private DiskWeapon weapon;

	public void Shoot( CameraComponent camera, Player player )
	{

	}
}
