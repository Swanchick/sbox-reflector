using Sandbox.Utility;
using System;


public sealed class Player : Component
{
	[Property]
	public PlayerMovement Movement { get; set; }

	[Property]
	public PlayerCameraMovement CameraMovement { get; set; }
	
	public PlayerMovementState playerMovementState { get; private set; } = PlayerMovementState.None;
	public bool Alive { get; set; } = true;
	public CharacterController playerController { get; private set; }
	public BoxCollider collider { get; private set; }

	public DiskWeapon DiskWeapon { get; set; }

	public bool IsSpectator { 
		get
		{
			return playerMovementState == PlayerMovementState.Noclip;
		} 
	}

	public string Name
	{
		get
		{
			return Network.Owner.DisplayName;
		}
	}

	public bool CanUseTrigger { get; set; } = true;

	[Sync]
	public Guid LastAttacker { get; set; }

	[Property]
	private GameObject ClientHUD;

	[Property]
	private GameObject playerHead;
	
	[Property]
	private CameraComponent playerCamera;

	[Property]
	private GameObject playerBody;

	[Property]
	private DiskWeapon diskWeapon;

	private Vector3 wishVelocity = Vector3.Zero;

	private float shakeSeed;

	private bool noclip = false;

	private bool grounded = false;

	public void Jump(Vector3 dir, float jumpForce)
	{
		playerController.Velocity = Vector3.Zero;
		playerController.Punch( dir * jumpForce );
	}
	
	public void Spectate(bool turn)
	{
		if ( IsProxy )
			return;

		if (turn)
		{
			playerController.Velocity = Vector3.Zero;

			playerMovementState = PlayerMovementState.Noclip;

			playerController.Enabled = false;
			playerBody.Enabled = false;
		}
		else
		{
			playerMovementState = PlayerMovementState.None;

			playerController.Enabled = true;
			playerBody.Enabled = true;

			playerController.Velocity = Vector3.Zero;
		}
	}

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();
		shakeSeed = Game.Random.Next();

		if ( IsProxy )
		{
			playerCamera.Destroy();
			ClientHUD.Destroy();
		} else
		{
			DiskWeapon = diskWeapon;
		}
	}
}
