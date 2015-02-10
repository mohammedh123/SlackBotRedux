using FluentMigrator;

namespace SlackBotRedux.DataMigrations.Migrations
{
    [Migration(003)]
    public class CreateVariableValuesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("VariableValues")
                  .WithColumn("VariableId").AsInt32().ForeignKey("Variables", "Id")
                  .WithColumn("Value").AsString()
                  .WithColumn("CreatedDate").AsCustom("datetimeoffset");
        }
    }
}