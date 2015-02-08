using FluentMigrator;

namespace SlackBotRedux.DataMigrations.Migrations
{
    [Migration(002)]
    public class CreateVariablesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Variables")
                  .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                  .WithColumn("Name").AsString(50)
                  .WithColumn("IsProtected").AsBoolean()
                  .WithColumn("CreatedDate").AsCustom("datetimeoffset")
                  .WithColumn("LastModifiedDate").AsCustom("datetimeoffset");
        }
    }
}
