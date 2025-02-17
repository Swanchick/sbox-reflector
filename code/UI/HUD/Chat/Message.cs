using System.Threading.Tasks;


public sealed class Message
{
	public string AuthorName { get; private set; }
	public string Content { get; private set; }

	public Color AuthorColor { get; set; } = new Color( 255, 136, 0 );

	public bool IsVisible { get; set; } = true;

	public string State => IsVisible ? "visible" : "hidden";

	public Message(string authorName, string content)
	{
		AuthorName = authorName;
		Content = content;
	}

	public async Task HideMessageAfterSomeTime(Chat chat)
	{
		await Task.Delay( 10000 );

		IsVisible = false;

		chat.StateHasChanged();
	}
}
