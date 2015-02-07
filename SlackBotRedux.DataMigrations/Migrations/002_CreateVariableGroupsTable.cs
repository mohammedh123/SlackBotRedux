using FluentMigrator;

namespace SlackBotRedux.DataMigrations.Migrations
{
    [Migration(002)]
    public class CreateVariableGroupsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("VariableGroups")
                  .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                  .WithColumn("Name").AsString(50);
        }
    }
}
