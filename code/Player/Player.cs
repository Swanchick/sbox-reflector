using System;


public sealed class Player : Component
{
	public PlayerMovement Movement { get; set; }

	public PlayerCameraMovement CameraMovement { get; set; }

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
