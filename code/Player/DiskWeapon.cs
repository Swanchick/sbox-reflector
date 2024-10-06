using Sandbox;


public class DiskWeapon : Component
{
	[Sync]
	public int diskCount { get; set; } = 0;

	[Property]
	private GameObject diskPrefab;
	[Property]
	private int maxDisks = 3;

	private List<Disk> playersDisks;

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
		
		GameObject diskObject = diskPrefab.Clone( WorldPosition + WorldRotation.Forward * 10f + Vector3.Down * 10f, Rotation.Identity );
		diskObject.NetworkSpawn();
		
		Disk disk = diskObject.GetComponent<Disk>();
		disk.Owner = GameObject.Parent.Id;
		disk.Direction = WorldRotation.Forward;

		diskCount++;
	}
}
