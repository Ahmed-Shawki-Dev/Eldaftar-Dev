namespace api.DTOs;

public record PaginationMetadata(int PageNumber, int PageSize, int TotalPages, int TotalRecords);

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = null,
    PaginationMetadata? Pagination = null,
    List<string>? Errors = null
)
    where T : class;
