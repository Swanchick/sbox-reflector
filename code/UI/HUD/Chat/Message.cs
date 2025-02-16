public sealed class Message
{
	public string AuthorName { get; private set; }
	public string Content { get; private set; }

	public Color AuthorColor { get; set; } = new Color( 255, 136, 0 );

	public Message(string authorName, string content)
	{
		AuthorName = authorName;
		Content = content;
	}
}
