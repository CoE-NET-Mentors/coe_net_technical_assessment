namespace TA_API.Models.Requests;

public record SupportTicketCreateRequest(string? Title, string? Description, string? Priority);
public record SupportTicketUpdateRequest(string? Status, string? AssigneeId);
