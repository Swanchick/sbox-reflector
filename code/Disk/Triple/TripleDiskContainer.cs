public class TripleDiskContainer
{
	private DiskThrower diskThrower;
	private int disks = 3;


	public TripleDiskContainer( DiskThrower diskThrower )
	{
		this.diskThrower = diskThrower;
	}

	public void ReturnDisk()
	{
		disks--;

		if ( disks <= 0 )
		{
			diskThrower.ReturnDisk();
		}
	}
}
