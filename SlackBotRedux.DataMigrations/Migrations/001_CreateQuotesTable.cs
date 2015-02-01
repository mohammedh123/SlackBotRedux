using FluentMigrator;

namespace SlackBotRedux.DataMigrations.Migrations
{
    [Migration(001)]
    public class CreateQuotesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Quotes")
                  .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                  .WithColumn("UserId").AsString()
                  .WithColumn("Text").AsString()
                  .WithColumn("Timestamp").AsCustom("datetimeoffset");
        }
    }
}
