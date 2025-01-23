namespace Youxel.Check.LicensesGenerator.Domain.Exceptions
{
    public class LicenseValidationException : Exception
    {
        public LicenseValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
