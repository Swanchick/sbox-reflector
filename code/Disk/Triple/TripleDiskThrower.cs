public class TripleDiskThrower : BaseDiskThrower
{
	[Property]
	private float diskRotation = 10f;

	protected override void OnShoot( Player player )
	{
		TripleDiskContainer container = new TripleDiskContainer( this );

		for (int i = -1; i <= 1; i++)
		{
			Vector3 dir = Weapon.WorldRotation.Forward;
			dir = dir.RotateAround( Vector3.Zero, new Angles( 0, i * diskRotation, 0 ) );
			
			GameObject diskObject = diskPrefab.Clone( Weapon.WorldPosition + dir * 10f + Vector3.Down * 10f, Rotation.Identity );
			diskObject.NetworkSpawn();

			TripleDisk disk = diskObject.GetComponent<TripleDisk>();
			disk.Container = container;
			disk.Setup( dir, player.GameObject.Id, this );
		}
	}
}
