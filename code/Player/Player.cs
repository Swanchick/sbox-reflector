using System;


public sealed class Player : Component
{
	[Property]
	public PlayerMovement Movement { get; set; }

	[Property]
	public PlayerCameraMovement CameraMovement { get; set; }

	[Property]
	public DiskWeapon DiskWeapon { get; set; }

	[Sync]
	public Guid LastAttacker { get; set; }

	public bool Alive { get; set; } = true;
	public BoxCollider collider { get; private set; }

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

	protected override void OnStart()
	{
		if ( IsProxy )
		{
			ClientHUD.Destroy();
		}
	}
}
