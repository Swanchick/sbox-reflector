public sealed class Message
{
	public Player Owner { get; set; }
	public string AuthorName { get; private set; }
	public string Content { get; private set; }


	public Message(Player owner, string content)
	{
		Owner = owner;
		AuthorName = Owner.Name;
		Content = content;
	}
}
