using System;


public sealed class Player : Component
{
	[Property]
	public PlayerMovement Movement { get; set; }

	[Property]
	public PlayerCameraMovement CameraMovement { get; set; }

	[Sync]
	public Guid LastAttacker { get; set; }

	public bool Alive { get; set; } = true;
	public BoxCollider collider { get; private set; }

	public DiskWeapon DiskWeapon { get; set; }

	public string Name
	{
		get
		{
			return Network.Owner.DisplayName;
		}
	}

	public bool CanUseTrigger { get; set; } = true;

	[Property]
	private GameObject ClientHUD;

	[Property]
	private DiskWeapon diskWeapon;


	protected override void OnStart()
	{
		if ( IsProxy )
		{
			ClientHUD.Destroy();
		} else
		{
			DiskWeapon = diskWeapon;
		}
	}
}
