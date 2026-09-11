namespace api.Interfaces;

public interface IBackupService
{
    Task<byte[]> ExportFullTeacherDataExcelAsync(Guid tenantId, Guid teacherId);
}
