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
	[Property]
	private int maxCollisions = 5;
	[Property]
	private float maxDistance = 1500f;

	[Property]
	private GameObject particleTrail;

	[Property]
	private GameObject diskModel;
	[Property]
	private float rotationSpeed = 50f;

	private CharacterController diskController;
	private int currentCollisions = 0;


	protected override void OnStart()
	{
		diskController = Components.Get<CharacterController>();
	}

	protected override void OnUpdate()
	{
		MoveDisk();
		GetCollision();
		GetCollisionWithPlayers();
		Alive();
		RotateDisk();
	}

	private void RotateDisk()
	{
		Angles diskAngles = diskModel.LocalRotation.Angles();
		diskAngles.yaw = -Time.Now * rotationSpeed;
		diskModel.LocalRotation = diskAngles.ToRotation();
	}

	private void Alive()
	{
		float distance = Vector3.DistanceBetween( WorldPosition, Vector3.Zero );

		if ( distance >= maxDistance )
		{
			PreDestroy();
			GameObject.Destroy();
		}	
	}

	private void MoveDisk()
	{
		diskController.Accelerate( Direction * diskSpeed );
		diskController.Move();
	}

	private Vector3 GetReflection( Vector3 direction, Vector3 normal )
	{
		return direction - (2 * Vector3.Dot( direction, normal ) * normal);
	}

	private void GetCollisionWithPlayers()
	{
		SceneTraceResult trace = Scene.Trace
			.Ray( WorldPosition, WorldPosition + Direction * collisionDistance )
			.Size( BBox.FromPositionAndSize(Vector3.Zero, collisionDistance) )
			.WithoutTags( "disk" )
			.Run();

		if ( !trace.Hit )
			return;

		GameObject gameObject = trace.GameObject;

		if ( !gameObject.Tags.Has( "player" ) )
			return;
		
		if ( gameObject.Id == Owner )
			return;

		PlayerMovement playerMovement = gameObject.Components.Get<PlayerMovement>();
		playerMovement.Jump( (Direction + Vector3.Up).Normal, collisionForce );
		playerMovement.Shake( 50f, 100f, new Vector3( 5, 5, 5 ), new Vector3( 10, 10, 10 ) );
	}

	private void PreDestroy()
	{
		if ( particleTrail == null )
			return;

		ParticleRingEmitter emitter = particleTrail.Components.Get<ParticleRingEmitter>();
		if ( emitter == null )
			return;

		emitter.Loop = false;
		emitter.DestroyOnEnd = true;

		particleTrail.SetParent( null );
	}

	protected override void OnDestroy()
	{
		GameObject owner = Scene.Directory.FindByGuid( Owner );
		DiskWeapon diskWeapon = owner.Components.GetInChildren<DiskWeapon>();

		if ( diskWeapon == null )
			return;

		diskWeapon.ReturnDisk();
	}

	private void GetCollision()
	{
		SceneTraceResult trace = Scene.Trace
			.Ray( WorldPosition, WorldPosition + Direction * collisionDistance )
			.Size( BBox.FromPositionAndSize( Vector3.Zero, 1 ) )
			.WithoutTags("disk", "player")
			.Run();

		if ( !trace.Hit )
			return;

		GameObject gameObject = trace.GameObject;
		Direction = GetReflection( Direction, trace.Normal );
		currentCollisions++;

		if (currentCollisions >= maxCollisions)
		{
			PreDestroy();
			
			GameObject.Destroy();
		}
	}
}
