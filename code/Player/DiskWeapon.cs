using Sandbox;


public class DiskWeapon : Component
{
	[Property]
	private GameObject diskPrefab;


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
		GameObject diskObject = diskPrefab.Clone( WorldPosition + WorldRotation.Forward * 10f + Vector3.Down * 10f, Rotation.Identity );
		diskObject.NetworkSpawn();

		
		Disk disk = diskObject.GetComponent<Disk>();
		disk.Owner = GameObject.Parent.Id;
		disk.Direction = WorldRotation.Forward;
	}
}
