namespace TA_API.Models.Requests;

public record ProductCreateRequest(string? Name, string? Sku, decimal? Price);
public record ProductUpdateRequest(string? Name, decimal? Price);
