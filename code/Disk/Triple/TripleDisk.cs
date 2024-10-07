public class TripleDisk : BaseDisk
{
	public TripleDiskContainer Container { get; set; }

	protected override void OnPreDestroy()
	{
		base.OnPreDestroy();

		Container.ReturnDisk();
	}
}
