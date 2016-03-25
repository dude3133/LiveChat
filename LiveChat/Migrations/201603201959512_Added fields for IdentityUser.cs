namespace LiveChat.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedfieldsforIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Banned", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreationTime", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Suspended", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Suspended");
            DropColumn("dbo.AspNetUsers", "CreationTime");
            DropColumn("dbo.AspNetUsers", "Banned");
        }
    }
}
