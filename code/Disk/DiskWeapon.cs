public class DiskWeapon : Component
{
	[Sync]
	public int diskCount { get; set; } = 0;

	[Property]
	private int maxDisks = 3;

	[Property]
	private Player player;

	[Property]
	private GameObject throwerSpace;

	[Property]
	private CameraComponent playerCamera;

	[Property]
	private BaseDiskThrower defaultDiskThrower;

	[Property]
	private int defaultDiskCount = 3;

	public List<BaseDiskThrower> DiskThrowers { get; set; } = new List<BaseDiskThrower>();

	public void ReturnDisk( GameObject diskThrower )
	{
		diskCount--;
		diskThrower.Destroy();
	}

	public bool CanAddThrower()
	{
		return DiskThrowers.Count < defaultDiskCount;
	}

	public void AddThrower(GameObject diskPrefab)
	{
		GameObject diskThrowerObject = diskPrefab.Clone(throwerSpace, Vector3.Zero, Rotation.Identity, Vector3.One);
		BaseDiskThrower diskThrower = diskThrowerObject.GetComponent<BaseDiskThrower>();
		if ( diskThrower == null )
			return;

		DiskThrowers.Add( diskThrower );
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

		BaseDiskThrower disk = defaultDiskThrower;

		bool isDefaultDisk = DiskThrowers.Count == 0;

		if ( !isDefaultDisk )
		{
			BaseDiskThrower lastDisk = DiskThrowers.First();
			DiskThrowers.RemoveAt(0);
			disk = lastDisk;
		}

		disk.Weapon = this;
		disk.Shoot( player );

		diskCount++;
	}
}
