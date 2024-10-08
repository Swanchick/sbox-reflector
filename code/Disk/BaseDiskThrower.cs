public abstract class BaseDiskThrower : Component, IWeapon
{
	public DiskWeapon Weapon { get; set; }
	
	[Property]
	protected GameObject diskPrefab;

	public void Shoot( Player player )
	{
		OnShoot( player );
	}

	public void ReturnDisk()
	{
		OnDiskReturn();
	}

	protected virtual void OnShoot( Player player )
	{
		Vector3 dir = Weapon.WorldRotation.Forward;

		GameObject diskObject = diskPrefab.Clone( Weapon.WorldPosition + dir * 10f + Vector3.Down * 10f, Rotation.Identity );
		diskObject.NetworkSpawn();

		BaseDisk disk = diskObject.GetComponent<BaseDisk>();
		disk.Setup( dir, player.GameObject.Id, this );
	}

	protected virtual void OnDiskReturn()
	{
		Weapon.ReturnDisk( GameObject );
	}
}
