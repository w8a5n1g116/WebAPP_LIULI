namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BaseDatas", "AllocationRate", c => c.Double(nullable: false));
            AddColumn("dbo.BaseDatas", "MonitorMoney", c => c.Double(nullable: false));
            DropColumn("dbo.BaseDatas", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BaseDatas", "Value", c => c.Double(nullable: false));
            DropColumn("dbo.BaseDatas", "MonitorMoney");
            DropColumn("dbo.BaseDatas", "AllocationRate");
        }
    }
}
