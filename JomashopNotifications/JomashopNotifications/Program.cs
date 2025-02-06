Console.WriteLine("Hello, World!");

// From Database
var links = new List<Uri>();
var results = JomashopService.ParseItemsFromLinksAsync(links);

await foreach (var result in results)
{
    if (result.IsLeft(out var item))
    {
        Console.WriteLine(item);
    }

    if (result.IsRight(out var browserDriverError))
    {
        Console.WriteLine($"An error occured while operating with a browser: {browserDriverError}");
    }
}

Console.Read();