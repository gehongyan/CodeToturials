namespace SystemTextJsonTests.Models;

public sealed record Person
{
    public Person(DateTimeOffset BirthDate)
    {
        this.BirthDate = BirthDate;
    }

    [JsonConverter(typeof(DateTimeOffsetTimestampConverter))]
    public DateTimeOffset BirthDate { get; init; }

    public void Deconstruct(out DateTimeOffset BirthDate)
    {
        BirthDate = this.BirthDate;
    }
}
