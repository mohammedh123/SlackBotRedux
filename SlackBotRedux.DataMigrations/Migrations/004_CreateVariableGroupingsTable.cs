using FluentMigrator;

namespace SlackBotRedux.DataMigrations.Migrations
{
    [Migration(004)]
    public class CreateVariableGroupingsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("VariableGroupings")
                  .WithColumn("VariableId").AsInt32().ForeignKey("Variables", "Id")
                  .WithColumn("GroupId").AsInt32().ForeignKey("Groups", "Id");
        }
    }
}
