using System;

public abstract class BaseDisk : Component
{
	[Sync]
	public Vector3 Direction { get; set; } = Vector3.Forward;

	[Sync]
	public Guid PlayerOwnerId { get; set; }

	[Property]
	protected float diskSpeed = 100f;
	[Property]
	protected float collisionForce = 600f;
	[Property]
	protected float collisionDistance = 50f;
	[Property]
	protected int maxCollisions = 5;
	[Property]
	protected float maxDistance = 1500f;

	[Property]
	protected float shakeMagnitude = 5f;

	[Property]
	protected GameObject particleTrail;

	[Property]
	protected GameObject diskModel;
	[Property]
	protected float rotationSpeed = 50f;

	protected CharacterController diskController;
	protected int currentCollisions = 0;

	protected Reflector reflector;

	private BaseDiskThrower baseDiskThrower;

	public void Setup( Vector3 direction, Guid OwnerId, BaseDiskThrower diskThrower )
	{
		Direction = direction;
		PlayerOwnerId = OwnerId;
		baseDiskThrower = diskThrower;
	}

	protected override void OnStart()
	{
		diskController = Components.Get<CharacterController>();

		GameObject reflectorObject = Scene.Directory.FindByName( "ReflectorManager" ).FirstOrDefault();
		if ( reflectorObject == null )
			return;

		reflector = reflectorObject.Components.Get<Reflector>();
	}

	protected override void OnUpdate()
	{
		MoveDisk();
		GetCollision();
		GetCollisionWithPlayers();
		Alive();
		DoProceduralAnimation();
	}

	private void DestroyDisk()
	{
		OnPreDestroy();
		GameObject.Destroy();
	}

	protected virtual void OnWallHit( SceneTraceResult trace )
	{
		GameObject gameObject = trace.GameObject;
		Direction = GetReflection( Direction, trace.Normal );
		currentCollisions++;

		if ( currentCollisions >= maxCollisions )
		{
			DestroyDisk();
		}
	}

	protected virtual void OnPlayerHit( SceneTraceResult trace, GameObject playerObject, Player player )
	{
		if ( playerObject.Id == PlayerOwnerId )
			return;

		player.Jump( (Direction + Vector3.Up).Normal, collisionForce );
		player.Shake( 
			50f, 
			100f, 
			new Vector3( shakeMagnitude, shakeMagnitude, shakeMagnitude ), 
			new Vector3( shakeMagnitude * 2, shakeMagnitude * 2, shakeMagnitude * 2 )
			);
	}

	protected virtual void OnDiskReturn()
	{
		baseDiskThrower.ReturnDisk();
	}

	protected virtual void OnPreDestroy()
	{
		if ( particleTrail == null )
			return;

		ParticleRingEmitter emitter = particleTrail.Components.Get<ParticleRingEmitter>();
		if ( emitter == null )
			return;

		emitter.Loop = false;
		emitter.DestroyOnEnd = true;

		particleTrail.SetParent( null );

		OnDiskReturn();
	}

	protected virtual void DoProceduralAnimation()
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
			DestroyDisk();
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

		Player player = gameObject.Components.Get<Player>();
		if ( player == null ) 
			return;

		reflector.OnPlayerHit( gameObject, player );
		OnPlayerHit( trace, gameObject, player );
	}

	private void GetCollision()
	{
		SceneTraceResult trace = Scene.Trace
			.Ray( WorldPosition, WorldPosition + Direction * collisionDistance )
			.WithoutTags("disk", "player")
			.Run();

		if ( !trace.Hit )
			return;

		OnWallHit( trace );
	}
}
