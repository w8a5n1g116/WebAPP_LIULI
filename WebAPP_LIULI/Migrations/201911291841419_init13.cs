namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerShortName", c => c.String());
            AddColumn("dbo.Customers", "Remarks", c => c.String());
            AddColumn("dbo.Orders", "DeliveryCount", c => c.Double(nullable: false));
            AddColumn("dbo.Drivers", "Remarks", c => c.String());
            AddColumn("dbo.Drivers", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.MaterialOrders", "OneOfTonPrice", c => c.Double(nullable: false));
            AddColumn("dbo.MaterialOrders", "IsComfirm", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Remarks", c => c.String());
            AddColumn("dbo.SendOrders", "OneOfTonPrice", c => c.Double(nullable: false));
            AddColumn("dbo.SendOrders", "IsComfirm", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Remarks");
            DropColumn("dbo.SendOrders", "IsComfirm");
            DropColumn("dbo.SendOrders", "OneOfTonPrice");
            DropColumn("dbo.Products", "Remarks");
            DropColumn("dbo.MaterialOrders", "IsComfirm");
            DropColumn("dbo.MaterialOrders", "OneOfTonPrice");
            DropColumn("dbo.Drivers", "Status");
            DropColumn("dbo.Drivers", "Remarks");
            DropColumn("dbo.Orders", "DeliveryCount");
            DropColumn("dbo.Customers", "Remarks");
            DropColumn("dbo.Customers", "CustomerShortName");
        }
    }
}
