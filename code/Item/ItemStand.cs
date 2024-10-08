using System.Threading.Tasks;

public class ItemStand : Component
{
	[Sync]
	public float CurrentTime { get; set; } = 0;


	[Sync]
	public bool IsItemTaken { get; set; } = false;

	[Property]
	private List<GameObject> Items;

	[Property]
	private float respawnTime = 5;

	[Property]
	private GameObject itemSpace;


	public void OnItemPickup()
	{
		IsItemTaken = true;
	}

	protected override void OnStart()
	{
		SpawnRandomItem();
	}

	protected override void OnUpdate()
	{
		if ( !IsItemTaken )
			return;

		CurrentTime += Time.Delta;

		if ( CurrentTime >= respawnTime)
		{
			IsItemTaken = false;
			CurrentTime = 0;

			SpawnRandomItem();
		}
	}

	private void SpawnRandomItem()
	{
		GameObject randomItem = Items[Game.Random.Next( 0, Items.Count - 1 )];
		if ( randomItem == null )
			return;

		GameObject itemObjectNew = randomItem.Clone( itemSpace.WorldPosition );
		itemObjectNew.NetworkSpawn();

		Item item = itemObjectNew.GetComponent<Item>();
		if ( item == null )
			return;

		item.ItemStandId = GameObject.Id;
	}
}
