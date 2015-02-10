namespace SlackBotRedux.Data.Interfaces
{
    public enum ValidateAddingValueResult
    {
        Success,
        AlreadyExists,
        Recursive
    }

    public interface IVariableRepository
    {
        ValidateAddingValueResult ValidateAddingValue(string variableName, string value);
    }
}