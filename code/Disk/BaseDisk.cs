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

	[Property]
	protected float timeToDestroy = 0.1f;

	protected CharacterController diskController;
	protected int currentCollisions = 0;

	private BaseDiskThrower baseDiskThrower;

	private bool isDestroying = false;
	private float currentTimeDestory = 0;

	public void Setup( Vector3 direction, Guid OwnerId, BaseDiskThrower diskThrower )
	{
		Direction = direction;
		PlayerOwnerId = OwnerId;
		baseDiskThrower = diskThrower;
	}

	protected Player GetPlayerOwner()
	{
		GameObject playerObject = Scene.Directory.FindByGuid( PlayerOwnerId );
		if ( playerObject == null )
			return null;

		return playerObject.Components.Get<Player>();
	}

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
		DoProceduralAnimation();
		DestroyThink();
	}

	private void DestroyThink()
	{
		if ( IsProxy )
			return;
		
		if ( !isDestroying )
			return;

		currentTimeDestory += Time.Delta;

		if ( currentTimeDestory > timeToDestroy )
		{
			OnDiskReturn();
			GameObject.Destroy();
		}
	}

	private void DestroyDisk()
	{
		if (isDestroying) 
			return;

		if ( particleTrail == null )
			return;

		ParticleRingEmitter emitter = particleTrail.Components.Get<ParticleRingEmitter>();
		if ( emitter == null )
			return;

		emitter.Loop = false;
		emitter.DestroyOnEnd = true;

		particleTrail.SetParent( null );

		diskModel.Enabled = false;
		
		OnPreDestroy();
		diskController.Enabled = false;
		isDestroying = true;
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

	[Rpc.Broadcast(NetFlags.HostOnly)]
	protected virtual void OnPlayerHit( SceneTraceResult trace, GameObject playerObject, Player player )
	{
		if ( playerObject.Id == PlayerOwnerId )
			return;

		player.Movement.Jump( (Direction + Vector3.Up * 0.5f).Normal, collisionForce );
		player.CameraMovement.Shake(
			50f,
			100f,
			new Vector3( shakeMagnitude, shakeMagnitude, shakeMagnitude ),
			new Vector3( shakeMagnitude * 2, shakeMagnitude * 2, shakeMagnitude * 2 )
			);

		DestroyDisk();
	}

	protected virtual void OnDiskReturn()
	{
		if ( baseDiskThrower == null )
			return;

		baseDiskThrower.ReturnDisk();
	}

	protected virtual void OnPreDestroy() { }

	protected virtual void DoProceduralAnimation()
	{
		if ( isDestroying ) 
			return;

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
		if (isDestroying) 
			return;

		diskController.Accelerate( Direction * diskSpeed );
		diskController.Move();
	}

	private Vector3 GetReflection( Vector3 direction, Vector3 normal )
	{
		return direction - (2 * Vector3.Dot( direction, normal ) * normal);
	}

	private void GetCollisionWithPlayers()
	{
		if ( isDestroying ) 
			return;

		SceneTraceResult trace = Scene.Trace
			.Ray( WorldPosition, WorldPosition + Direction * collisionDistance / 3 )
			.Size( BBox.FromPositionAndSize(Vector3.Zero, collisionDistance / 3) )
			.WithoutTags( "disk" )
			.Run();

		if ( !trace.Hit )
			return;

		GameObject gameObject = trace.GameObject;

		if ( !gameObject.Tags.Has( "player" ) )
			return;

		Player victim = gameObject.Components.Get<Player>();
		if ( victim == null ) 
			return;

		Player playerOwner = GetPlayerOwner();
		if ( playerOwner == null ) 
			return;

		Scene.RunEvent<IReflector>( x => x.OnPlayerHit( playerOwner, victim ) );
		
		OnPlayerHit( trace, gameObject, victim );
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
