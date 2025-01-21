namespace Youxel.Check.LicensesGenerator.Infrastructure.Repositories
{
    public interface IAppSettingsRepository
    {
        string GetByKey(string key);
    }
}
