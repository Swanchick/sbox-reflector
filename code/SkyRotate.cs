public class SkyRotate : Component
{
	[Property]
	private float rotatingSpeed = 0.5f;


	protected override void OnFixedUpdate()
	{
		Angles angles = WorldRotation.Angles();
		angles.pitch = Time.Now * rotatingSpeed;
		angles.yaw = Time.Now * rotatingSpeed;

		WorldRotation = angles;

	}
}
