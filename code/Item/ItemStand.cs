using System.Threading.Tasks;

public class ItemStand : Component
{
	[Property]
	private List<GameObject> Items;

	[Property]
	private float respawnTime = 5;

	[Property]
	private GameObject itemSpace;

	[Broadcast]
	public void OnItemPickup()
	{
		_ = WaitAndSpawn();
	}

	private async Task WaitAndSpawn()
	{
		await Task.DelaySeconds( respawnTime );

		SpawnRandomItem();
	}

	protected override void OnStart()
	{
		if ( IsProxy )
			return;

		SpawnRandomItem();
	}

	private void SpawnRandomItem()
	{
		if ( IsProxy )
			return;

		GameObject randomItem = Items[Game.Random.Next( 0, Items.Count - 1 )];
		if ( randomItem == null )
			return;

		GameObject itemObjectNew = randomItem.Clone( itemSpace.WorldPosition );
		itemObjectNew.NetworkSpawn( Network.Owner );

		Item item = itemObjectNew.GetComponent<Item>();
		if ( item == null )
			return;

		item.ItemStandId = GameObject.Id;
	}
}
