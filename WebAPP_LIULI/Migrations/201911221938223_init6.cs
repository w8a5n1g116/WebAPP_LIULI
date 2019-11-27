namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Warehousings", "InspectionCount", c => c.Double(nullable: false));
            AddColumn("dbo.Warehousings", "InspectionUserName", c => c.String());
            AddColumn("dbo.Warehousings", "InspectionTime", c => c.DateTime());
            AddColumn("dbo.Warehousings", "WarehousingName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehousings", "WarehousingName");
            DropColumn("dbo.Warehousings", "InspectionTime");
            DropColumn("dbo.Warehousings", "InspectionUserName");
            DropColumn("dbo.Warehousings", "InspectionCount");
        }
    }
}
