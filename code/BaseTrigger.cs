public class BaseTrigger : Component, Component.ITriggerListener
{
	[Property]
	protected bool activateInNoclip = false;

	protected virtual void OnPlayerEnter( Player player ) { }

	protected virtual void OnPlayerLeave( Player player ) { }

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( !other.Tags.Has( "player" ) )
			return;

		Player player = other.GetComponent<Player>();
		if ( player == null )
			return;

		if ( player.Movement.IsSpectator && !activateInNoclip )
			return;

		if ( !player.CanUseTrigger )
			return;

		OnPlayerEnter( player );
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		if ( !other.Tags.Has( "player" ) )
			return;

		Player player = other.GetComponent<Player>();
		if ( player == null ) 
			return;

		if ( player.Movement.IsSpectator && !activateInNoclip )
			return;

		if ( !player.CanUseTrigger )
			return;

		OnPlayerLeave( player );
	}
}
