using System.Threading.Tasks;

public class ItemStand : Component
{
	[Property]
	private List<GameObject> Items;

	[Property]
	private float respawnTime = 5;

	[Property]
	private GameObject itemSpace;

	private bool isItemTaken = false;

	private float currentTime = 0;

	public void OnItemPickup()
	{
		isItemTaken = true;
	}

	protected override void OnStart()
	{
		SpawnRandomItem();
	}

	protected override void OnUpdate()
	{
		if ( !isItemTaken )
			return;

		currentTime += Time.Delta;

		if (currentTime >= respawnTime)
		{
			isItemTaken = false;
			currentTime = 0;

			SpawnRandomItem();
		}
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
