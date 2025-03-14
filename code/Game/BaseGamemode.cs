using System;
using System.Threading.Tasks;


public abstract class BaseGamemode : Component, IGamemode
{
	[Property]
	protected string name = "Base Gamemode";

	[Property]
	protected string description = "Base Gamemode Description";

	public string Name => name;
	public string Description => description;

	public virtual void OnGameStart() { }
	
	public virtual void OnRoundStart() { }
	
	public virtual void OnRoundEnd() { }
	
	public virtual void OnPlayerJoin( Player player )
	{
		PlayerManager pm = PlayerManager.instance;
		if ( !pm.IsValid() )
			return;

		pm.SayMessage( $"{ player.Name } has joined the game!" );

		if ( !pm.LocalPlayer.IsValid() )
			return;

		pm.LocalPlayer.ClientHUD.Scoreboard.StateHasChanged();
	}

	public virtual void OnPlayerLeave( Player player )
	{
		PlayerManager pm = PlayerManager.instance;

		pm.SayMessage( $"{player.Name} has left the game!" );
		pm.PlayerIds.Remove( player.GameObject.Id );

		pm.LocalPlayer.ClientHUD.Scoreboard.StateHasChanged();
	}

	public virtual void OnPlayerSpawn( Player player ) { }

	public virtual void OnPlayerDeath( Player player )
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		player.LastAttacker = Guid.Empty;

		player.Spectate();

		_ = SpectatorWait( player );
	}

	protected void PlayerSpawn( Player player )
	{
		Reflector reflector = Reflector.instance;
		if ( !reflector.IsValid() )
			return;

		player.Transform.ClearInterpolation();
		player.Transform.World = reflector.GetRandomSpawnpoint();
		player.Movement.PlayerController.Velocity = Vector3.Zero;
		player.Transform.ClearInterpolation();

		player.Spawn();
	}

	private async Task SpectatorWait( Player player )
	{
		await Task.DelaySeconds( 5 );

		PlayerSpawn( player );
	}

	public virtual void OnPlayeKill( Player victim )
	{
		PlayerManager pm = PlayerManager.instance;
		if ( !pm.IsValid() )
			return;

		if ( !victim.IsValid() || !pm.LocalPlayer.IsValid() )
			return;

		pm.LocalPlayer.ClientHUD.KillFeed.AddKill( "", victim.Name );
	}

	public virtual void OnPlayerKill( Player attacker, Player victim )
	{
		PlayerManager pm = PlayerManager.instance;
		if ( !pm.IsValid() )
			return;

		if ( !attacker.IsValid() || !victim.IsValid() || !pm.LocalPlayer.IsValid() )
			return;

		pm.LocalPlayer.ClientHUD.KillFeed.AddKill( attacker.Name, victim.Name );
	}

	public virtual void OnPlayerHit( Player attacker, Player victim ) { }

	public virtual bool AllowSpawn( Player player )
	{
		return true;
	}

	public virtual bool AllowPlayerDeath( Player player )
	{
		return true;
	}
}
