using api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    // 1. Success With Message
    protected IActionResult Success<T>(T data, string message = "")
        where T : class
    {
        return Ok(new ApiResponse<T>(true, message, Data: data));
    }

    // 2. Success With Pagination
    protected IActionResult Success<T>(T data, PaginationMetadata pagination, string message = "")
        where T : class
    {
        return Ok(new ApiResponse<T>(true, message, Data: data, Pagination: pagination));
    }

    // 3. Success Without Data
    protected IActionResult Success(string message = "")
    {
        return Ok(new ApiResponse<object>(true, message));
    }

    // 4. Create New Data
    protected IActionResult Create<T>(
        string actionName,
        object? routeValues,
        T data,
        string message = ""
    )
        where T : class
    {
        var res = new ApiResponse<T>(true, message, Data: data);
        return CreatedAtAction(actionName, routeValues, res);
    }

    // 5. Bad Request
    protected IActionResult BadReq(string message, List<string>? errors = null)
    {
        return BadRequest(new ApiResponse<object>(false, message, Errors: errors));
    }

    // 6. Not Found
    protected IActionResult NotFoundRes(string message, List<string>? errors = null)
    {
        return NotFound(new ApiResponse<object>(false, message, Errors: errors));
    }

    // 7. Unauthorized
    protected IActionResult UnauthorizedRes(string message = "")
    {
        return Unauthorized(new ApiResponse<object>(false, message));
    }

    // 8. Forbidden
    protected IActionResult ForbiddenRes(string message = "")
    {
        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>(false, message));
    }
}
