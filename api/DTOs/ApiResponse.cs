public record PaginationMetadata(int PageNumber, int PageSize, int TotalPages, int TotalRecords)
{
    public static PaginationMetadata Create(int pageNumber, int pageSize, int totalRecords)
    {
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        return new PaginationMetadata(pageNumber, pageSize, totalPages, totalRecords);
    }
}

public record ApiResponse<T>(
    bool Success,
    string Message = "",
    T? Data = default,
    PaginationMetadata? Pagination = null,
    List<string>? Errors = null
)
    where T : class
{
    public static ApiResponse<T> Ok(T data, string message = "") => new(true, message, Data: data);

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new(false, message, Errors: errors);

    public static ApiResponse<T> OkPaged(
        T data,
        PaginationMetadata pagination,
        string message = ""
    ) => new(true, message, Data: data, Pagination: pagination);
}
