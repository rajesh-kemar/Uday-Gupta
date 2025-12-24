public class WarehouseFilterModel
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public long? MinCapacity { get; set; }
    public long? MaxCapacity { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}