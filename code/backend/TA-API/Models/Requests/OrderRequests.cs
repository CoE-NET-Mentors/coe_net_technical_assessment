namespace TA_API.Models.Requests;

public record OrderCreateRequest(string? CustomerId, string? Status);
public record OrderUpdateRequest(string? Status);
