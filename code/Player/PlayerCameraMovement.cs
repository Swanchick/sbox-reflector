using Sandbox.Utility;
using System;


public sealed class PlayerCameraMovement : Component 
{
	[Property]
	public Player Player { get; set; }

	[Property]
	private float playerCameraRotation = 5f;
	
	[Property]
	private float playerCameraSpeed = 10f;

	[Property]
	private float cameraSensitivity = 0.1f;

	[Property]
	private GameObject playerHead;

	[Property]
	private CameraComponent playerCamera;

	[Property]
	private GameObject playerBody;
	
	[Property]
	private GameObject playerShake;

	[Property]
	private float shakeRecoverySpeed = 1f;

	private float shakeTrauma = 0;
	private Vector3 shakeMax;
	private Vector3 shakeAnglesMax;
	private float shakeFrequecy;
	private float shakeSeed;

	public void Shake(float trauma, float frequency, Vector3 maxPos, Vector3 maxAngle)
	{
		shakeTrauma = trauma;
		shakeFrequecy = frequency;
		shakeMax = maxPos;
		shakeAnglesMax = maxAngle;
	}

	private CharacterController playerController;

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();
	}

	protected override void OnUpdate()
	{
		CameraRotation();
		CameraMovement();
		Shaking();
		RotateBody();
	}

	private void CameraRotation()
	{
		if ( IsProxy )
			return;

		Vector2 mouseDelta = Input.MouseDelta;

		Angles headAngles = playerHead.LocalRotation.Angles();
		Angles cameraAngles = playerCamera.LocalRotation.Angles();

		headAngles.yaw -= mouseDelta.x * cameraSensitivity;

		float pitchAngle = cameraAngles.pitch + mouseDelta.y * cameraSensitivity;
		cameraAngles.pitch = Math.Clamp( pitchAngle, -89f, 89f );

		playerHead.LocalRotation = headAngles.ToRotation();
		playerCamera.LocalRotation = cameraAngles.ToRotation();
	}

	private void RotateBody()
	{
		playerBody.LocalRotation = playerHead.LocalRotation;
	}


	private void Shaking()
	{
		if ( IsProxy )
			return;

		if ( shakeTrauma == 0 )
			return;

		Vector3 shakeVelocity = new Vector3(
			shakeMax.x * (Noise.Perlin( shakeSeed, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeMax.y * (Noise.Perlin( shakeSeed + 1, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeMax.z * (Noise.Perlin( shakeSeed + 2, Time.Now * shakeFrequecy ) * 2 - 1)
			);

		shakeVelocity *= shakeTrauma;

		Angles shakeRotationalVelocity = new Angles(
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 3, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 4, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 5, Time.Now * shakeFrequecy ) * 2 - 1)
			);

		shakeRotationalVelocity *= shakeTrauma;

		playerShake.LocalPosition = Vector3.Lerp( playerShake.LocalPosition, shakeVelocity, Time.Delta * playerCameraSpeed );
		playerShake.LocalRotation = Rotation.Lerp( playerShake.LocalRotation, shakeRotationalVelocity.ToRotation(), Time.Delta * playerCameraSpeed );

		shakeTrauma = Math.Clamp( shakeTrauma - shakeRecoverySpeed * Time.Delta, 0, 1 );
	}

	private void CameraMovement()
	{
		if ( IsProxy )
			return;

		float dir = (Input.Down( "Right" ) ? 1 : 0) - (Input.Down( "Left" ) ? 1 : 0);

		Angles playerAngles = playerCamera.LocalRotation.Angles();

		playerAngles.roll = dir * playerCameraRotation;
		Rotation playerRotation = playerAngles.ToRotation();

		Rotation defaultRotation = new Angles( playerAngles.pitch, 0, 0 );

		playerCamera.LocalRotation = Rotation.Lerp( 
			playerCamera.LocalRotation, 
			((playerController.IsOnGround && playerController.Enabled) || !Player.Movement.IsSpectator) ? playerRotation : defaultRotation, 
			Time.Delta * playerCameraSpeed 
		);
	}
}
