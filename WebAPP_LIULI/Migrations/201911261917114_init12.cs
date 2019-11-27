namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderPrice");
        }
    }
}
