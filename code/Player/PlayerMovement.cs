using Sandbox;


public class PlayerMovement : Component
{
	private CharacterController characterController;

	protected override void OnStart()
	{
		characterController = Components.Get<CharacterController>();

		base.OnStart();
	}

	protected override void OnUpdate()
	{
		

		base.OnUpdate();
	}

	protected override void OnFixedUpdate()
	{
		Movement();

		base.OnFixedUpdate();
	}

	private void Movement()
	{
		
	}
}
