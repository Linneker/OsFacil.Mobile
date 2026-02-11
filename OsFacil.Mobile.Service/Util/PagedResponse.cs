namespace OsFacil.Mobile.Service.Util;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    PageMeta Meta
);