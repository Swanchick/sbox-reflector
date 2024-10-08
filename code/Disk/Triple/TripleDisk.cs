public class TripleDisk : BaseDisk
{
	public TripleDiskContainer Container { get; set; }

	protected override void OnDiskReturn()
	{
		Container.ReturnDisk();
	}
}
