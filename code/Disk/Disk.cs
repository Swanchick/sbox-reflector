using Sandbox;
using System;

public class Disk : Component
{
	[Sync]
	public Vector3 Direction { get; set; } = Vector3.Forward;

	[Sync]
	public Guid Owner { get; set; }

	[Property]
	private float diskSpeed = 100f;

	[Property]
	private float collisionForce = 600f;

	[Property]
	private float collisionDistance = 50f;

	private CharacterController diskController;


	protected override void OnStart()
	{
		diskController = Components.Get<CharacterController>();
	}

	protected override void OnUpdate()
	{
		MoveDisk();

		GetCollision();
	}

	private void MoveDisk()
	{
		diskController.Velocity = Direction * diskSpeed;
		diskController.Move();
	}

	private Vector3 GetReflection(Vector3 direction, Vector3 normal)
	{
		return direction - (2 * Vector3.Dot( direction, normal ) * normal);
	}

	private void GetCollision()
	{
		SceneTraceResult trace = Scene.Trace
			.Ray( WorldPosition, WorldPosition + Direction * collisionDistance )
			.Run();

		Gizmo.Draw.Arrow( WorldPosition, WorldPosition + Direction * collisionDistance );

		if ( !trace.Hit )
			return;

		GameObject gameObject = trace.GameObject;

		if ( gameObject.Tags.Has( "player" ) )
		{
			if ( gameObject.Id == Owner )
				return;

			PlayerMovement playerMovement = gameObject.Components.Get<PlayerMovement>();
			playerMovement.Jump( (Direction + Vector3.Up).Normal, collisionForce );

			return;
		}

		Direction = GetReflection( Direction, trace.Normal );
	}
}
