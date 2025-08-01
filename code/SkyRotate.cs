public class SkyRotate : Component
{
	[Property]
	private float rotatingSpeed = 0.5f;


	protected override void OnFixedUpdate()
	{
		Angles angles = WorldRotation.Angles();
		angles.pitch += Time.Delta * rotatingSpeed;
		angles.yaw += Time.Delta * rotatingSpeed;

		WorldRotation = angles;
	}
}
