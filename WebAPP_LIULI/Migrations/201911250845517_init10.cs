namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RewardPunishments", "UserName", c => c.String());
            AddColumn("dbo.TeamAllocations", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamAllocations", "UserName");
            DropColumn("dbo.RewardPunishments", "UserName");
        }
    }
}
