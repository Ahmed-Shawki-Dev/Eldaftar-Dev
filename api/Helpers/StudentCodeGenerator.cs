namespace api.Helpers;

public static class StudentCodeGenerator
{
    public static string GenerateRandomStudentCode()
    {
        return Random.Shared.Next(100000, 1000000).ToString();
    }
}
