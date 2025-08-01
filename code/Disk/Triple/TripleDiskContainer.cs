public class TripleDiskContainer
{
	private TripleDiskThrower diskThrower;
	private int disks = 3;


	public TripleDiskContainer( TripleDiskThrower diskThrower )
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
