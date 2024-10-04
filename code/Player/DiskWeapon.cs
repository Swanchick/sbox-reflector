using Sandbox;


public class DiskWeapon : Component
{
	protected override void OnUpdate()
	{
		GetShoot();
	}

	private void GetShoot()
	{
		if ( Input.Pressed("attack1") )
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		Log.Info("Hello World");
	}
}
