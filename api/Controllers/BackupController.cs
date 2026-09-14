using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/backup")]
public class BackupController(IBackupService backupService) : BaseApiController
{
    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportTeacherDataAsExcel(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        byte[] fileBytes = await backupService.ExportFullTeacherDataExcelAsync(
            userContext.TenantId,
            teacherId
        );

        const string contentType =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = $"Teacher-Data-{DateTime.UtcNow:yyyyMMdd-HHmm}.xlsx";

        return File(fileBytes, contentType, fileName);
    }
}
