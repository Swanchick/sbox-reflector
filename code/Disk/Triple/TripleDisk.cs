public class TripleDisk : BaseDisk
{
	public TripleDiskContainer Container { get; set; }

	protected override void OnDiskReturn()
	{
		if ( Container == null )
			return;

		Container.ReturnDisk();
	}
}
