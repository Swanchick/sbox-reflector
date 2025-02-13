public class DiskWeapon : Component
{
	[Sync]
	public int diskCount { get; set; } = 0;

	public List<BaseDiskThrower> DiskThrowers { get; set; } = new List<BaseDiskThrower>();

	public Vector3 ShootPos 
	{
		get 
		{
			return throwerSpace.WorldPosition;
		}
	}

	public Vector3 ShootDir 
	{
		get 
		{
			return throwerSpace.WorldRotation.Forward;
		}
	}

	[Property]
	private Player Player;

	[Property]
	private int maxDisks = 3;

	[Property]
	private GameObject throwerSpace;

	private BaseDiskThrower defaultDiskThrower;

	[Property]
	private int defaultDiskCount = 3;

	[Property]
	private PlayerHUD playerHUD;

	public void ReturnDisk( GameObject diskThrower )
	{
		diskCount--;

		playerHUD?.ChangeDisks( maxDisks - diskCount );

		if ( diskThrower.Id == defaultDiskThrower.GameObject.Id )
			return;
		
		diskThrower.Destroy();
	}

	public bool CanAddThrower()
	{
		return DiskThrowers.Count < defaultDiskCount;
	}

	public void AddThrower( GameObject diskPrefab )
	{
		GameObject diskThrowerObject = diskPrefab.Clone(GameObject, Vector3.Zero, Rotation.Identity, Vector3.One);
		diskThrowerObject.NetworkSpawn();

		BaseDiskThrower diskThrower = diskThrowerObject.Components.Get<BaseDiskThrower>();

		if ( diskThrower == null )
			return;

		DiskThrowers.Add( diskThrower );
	}

	protected override void OnStart()
	{
		
		defaultDiskThrower = Components.GetInChildren<DefaultDiskThrower>();
	}

	protected override void OnUpdate()
	{
		GetShoot();
	}

	private void GetShoot()
	{
		if ( IsProxy )
			return;

		if ( Input.Pressed("attack1") && !Player.Movement.IsSpectator )
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
			BaseDiskThrower lastDisk = DiskThrowers[0];
			DiskThrowers.RemoveAt(0);

			disk = lastDisk;
		}

		disk.Weapon = this;
		disk.Shoot( Player );

		diskCount++;

		playerHUD?.ChangeDisks( maxDisks - diskCount );
	}
}
