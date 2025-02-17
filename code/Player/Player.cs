using System;


public sealed class Player : Component
{
	public PlayerMovement Movement { get; set; }

	public PlayerCameraMovement CameraMovement { get; set; }

	public DiskWeapon DiskWeapon { get; set; }

	public PlayerHUD ClientHUD { get; set; }

	public PlayerStats Stats { get; set; }

	[Sync]
	public Guid LastAttacker { get; set; }

	public string Name => Network.Owner.DisplayName;
	public string SteamId => Network.Owner.SteamId.ToString();
	public Connection Connection => Network.Owner;

	public enum State 
	{
		Alive,
		Dead,
		Spectator,
	}

	public State CurrentState { get; set; } = State.Alive;

	public bool IsAlive => CurrentState == State.Alive;
	public bool IsDead => CurrentState == State.Dead;
	public bool IsSpectator => CurrentState == State.Spectator;

	protected override void OnStart()
	{
		Movement = Components.Get<PlayerMovement>();
		CameraMovement = Components.Get<PlayerCameraMovement>();
		DiskWeapon = Components.Get<DiskWeapon>();
		ClientHUD = Components.GetInChildren<PlayerHUD>();
		Stats = Components.Get<PlayerStats>();

		if ( IsProxy )
		{
			ClientHUD.Destroy();
		}
	}
}
