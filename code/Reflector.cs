public class Reflector : Component
{
	[Property]
	private string test;
	
	private static Reflector instance;

	public void Test()
	{
		Log.Info( test );
	}
}
