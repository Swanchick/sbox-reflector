using Sandbox;


public class DiskWeapon : Component
{
	[Sync]
	public int diskCount { get; set; } = 0;

	[Property]
	private int maxDisks = 3;

	[Property]
	private Player player;
	[Property]
	private CameraComponent playerCamera;

	[Property]
	private BaseDiskThrower diskThrower;

	public void ReturnDisk()
	{
		diskCount--;
	}

	protected override void OnUpdate()
	{
		GetShoot();
	}

	private void GetShoot()
	{
		if ( IsProxy )
			return;

		if ( Input.Pressed("attack1") )
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		if ( diskCount >= maxDisks )
			return;

		diskThrower.Weapon = this;
		diskThrower.Shoot( player );

		diskCount++;
	}
}
