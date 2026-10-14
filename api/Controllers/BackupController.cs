using api.Filters;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
[Route("api/{slug}/backup")]
[ServiceFilter(typeof(CheckTeacherTenantFilter))]
public class BackupController(IBackupService backupService) : BaseApiController
{
    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportTeacherDataAsExcel([FromRoute] string slug)
    {
        var userContext = (UserContext)HttpContext.Items["UserContext"]!;

        byte[] fileBytes = await backupService.ExportFullTeacherDataExcelAsync(
            userContext.TenantId,
            userContext.TeacherId
        );

        const string contentType =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = $"Teacher-Data-{DateTime.UtcNow:yyyyMMdd-HHmm}.xlsx";

        return File(fileBytes, contentType, fileName);
    }
}
