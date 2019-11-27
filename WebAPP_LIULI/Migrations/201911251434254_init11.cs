namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DingUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DingUserId");
        }
    }
}
