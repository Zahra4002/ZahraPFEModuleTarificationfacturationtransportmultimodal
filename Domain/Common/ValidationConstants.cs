namespace Application.Common.Constants
{
    public static class ValidationConstants
    {
        public const string ValidationErrors = "Validation errors : ";
        public const string FirstNameMustHasValue = "FirstName must have a value : ";
        public const string CompanyNameMustHasValue = "CompanyName must have a value : ";
        public const string ContactMustHasValue = "Contact must have a value: ";
        public const string InvoiceNumberMustHasValue = "InvoiceNumber must have a value : ";
       

        public const string ClientIdMustHasValue = "ClientId must have a value : ";
        public const string IssueDateMustHasValue = "IssueDate must have a value : ";
        public const string DueDateMustHasValue = "DueDate must have a value : ";
        public const string TotalTTCMustBeGreaterThanZero = "TotalTTC must be greater than zero : ";
        public const string StatusMustBeValid = "Status must be valid : ";

        // Ajoutées pour corriger CS0117 utilisé par UpdateInvoiceCommandValidator
        public const string IdMustHasValue = "Id must have a value : ";
        public const string ExchangeRateMustBeGreaterThanZero = "ExchangeRate must be greater than zero : ";
    }
}
