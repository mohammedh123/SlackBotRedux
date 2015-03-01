namespace SlackBotRedux.Configuration
{
    public interface IVariableConfiguration
    {
        string PrefixString { get; set; }
        string AllowedNameCharactersRegex { get; set; }
        string InvalidNameCharactersRegex { get; set; }
    }

    public class VariableConfiguration : IVariableConfiguration
    {
        public string PrefixString { get; set; }
        public string AllowedNameCharactersRegex { get; set; }
        public string InvalidNameCharactersRegex { get; set; }
    }
}
