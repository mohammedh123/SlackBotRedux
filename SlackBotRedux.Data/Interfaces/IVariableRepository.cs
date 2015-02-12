namespace SlackBotRedux.Data.Interfaces
{
    public enum ValidateAddingValueResult
    {
        Success,
        AlreadyExists,
        Recursive
    }

    public enum ValidateDeletingValueResult
    {
        Success,
        Recursive
    }

    public interface IVariableRepository
    {
        ValidateAddingValueResult ValidateAddingValue(string variableName, string value);
        ValidateDeletingValueResult ValidateDeletingValue(string variableName, string value);
    }
}