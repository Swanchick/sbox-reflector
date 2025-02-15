﻿using System;


public sealed class Player : Component
{
	public PlayerMovement Movement { get; set; }

	public PlayerCameraMovement CameraMovement { get; set; }

	public DiskWeapon DiskWeapon { get; set; }

	public PlayerHUD ClientHUD { get; set; }

	[Sync]
	public Guid LastAttacker { get; set; }

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

	protected override void OnStart()
	{
		Movement = Components.Get<PlayerMovement>();
		CameraMovement = Components.Get<PlayerCameraMovement>();
		DiskWeapon = Components.Get<DiskWeapon>();
		ClientHUD = Components.GetInChildren<PlayerHUD>();

		if ( IsProxy )
		{
			ClientHUD.Destroy();
		}
	}
}
