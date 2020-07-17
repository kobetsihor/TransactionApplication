namespace TransactionApplication.BL.Constants
{
    public static class ValidationMessages
    {
        public const string MaxLengthError = "Text Length more than 50";
        public const string DecimalTypeError = "It's not a decimal";
        public const string CodeFormatError = "It's not ISO4217 format";
        public const string EmptyItemError = "Item should not be empty";
        public const string DateFormatError = "Date can't be empty and should be with correct format with correct format";
        public const string InappropriateStatusError = "Inappropriate Status";
    }
}