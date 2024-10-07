public abstract class BaseDiskThrower : Component, IWeapon
{
	public DiskWeapon Weapon { get; set; }
	
	[Property]
	protected GameObject diskPrefab;

	protected virtual void OnShoot( Player player )
	{
		Vector3 dir = Weapon.WorldRotation.Forward;

		GameObject diskObject = diskPrefab.Clone( Weapon.WorldPosition + dir * 10f + Vector3.Down * 10f, Rotation.Identity );
		diskObject.NetworkSpawn();

		BaseDisk disk = diskObject.GetComponent<BaseDisk>();
		disk.Setup( dir, GameObject.Parent.Id );
	}

	public void Shoot( Player player )
	{
		OnShoot( player );
	}
}
