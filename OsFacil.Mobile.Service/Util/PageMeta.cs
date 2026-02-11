namespace OsFacil.Mobile.Service.Util;

public sealed record PageMeta(
    int PageNumber,
    int PageSize,
    long TotalItems
)
{
    public int TotalPages { get => TotalItems == 0 ? 0 : (int)((TotalItems + PageSize - 1) / PageSize); }
    public bool HasNext { get => TotalItems == 0 ? false : TotalPages > PageNumber; }
    public bool HasPrevious { get => TotalItems == 0 ? false : TotalPages < PageNumber; }
}
