namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Warehousings", "CustomerShortName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehousings", "CustomerShortName");
        }
    }
}
