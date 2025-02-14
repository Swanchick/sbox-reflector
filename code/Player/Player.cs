using System;


public sealed class Player : Component
{
	public PlayerMovement Movement { get; set; }

	public PlayerCameraMovement CameraMovement { get; set; }

	public DiskWeapon DiskWeapon { get; set; }

	[Sync]
	public Guid LastAttacker { get; set; }

	public bool Alive { get; set; } = true;

	public string Name
	{
		get
		{
			return Network.Owner.DisplayName;
		}
	}

	public enum State 
	{
		Alive,
		Dead,
		Spectator,
	}

	public State CurrentState { get; set; } = State.Alive;

	public bool IsAlive 
	{
		get
		{
			return CurrentState == State.Alive;
		}
	}

	public bool IsDead
	{
		get
		{
			return CurrentState == State.Dead;
		}
	}

	public bool IsSpectator
	{
		get
		{
			return CurrentState == State.Spectator;
		}
	}

	[Property]
	private GameObject ClientHUD;

	protected override void OnStart()
	{
		Movement = Components.Get<PlayerMovement>();
		CameraMovement = Components.Get<PlayerCameraMovement>();
		DiskWeapon = Components.Get<DiskWeapon>();


		if ( IsProxy )
		{
			ClientHUD.Destroy();
		}
	}
}
