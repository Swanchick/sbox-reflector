using Sandbox;
using System;


public class TestComponent : Component
{
	[Property] private string text;

	protected override void OnStart()
	{
		Log.Info( text );

		base.OnStart();
	}
}
