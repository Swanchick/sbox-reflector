using System.Threading.Tasks;

public class ItemStand : Component
{
	[Property]
	private List<GameObject> Items;

	[Property]
	private int respawnTime = 5000;

	[Property]
	private GameObject itemSpace;

	public void OnItemPickup()
	{
		_ = WaitAndSpawn();
	}

	protected override void OnStart()
	{
		SpawnRandomItem();
	}

	private async Task WaitAndSpawn()
	{
		await Task.Delay(respawnTime);

		SpawnRandomItem();
	}

	private void SpawnRandomItem()
	{
		GameObject itemObject = Items[Game.Random.Next( 0, Items.Count - 1 )];
		if ( itemObject == null )
			return;

		GameObject itemObjectNew = itemObject.Clone( itemSpace, Vector3.Zero, Rotation.Identity, Vector3.One );
		itemObjectNew.NetworkSpawn();

		Item item = itemObjectNew.GetComponent<Item>();
		if ( item == null )
			return;

		item.ItemStand = this;
	}
}
