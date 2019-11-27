namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamAllocations", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MaterialOrders", "SendTime", c => c.DateTime());
            AlterColumn("dbo.MaterialOrders", "ReceiveTime", c => c.DateTime());
            AlterColumn("dbo.SendOrders", "SendTime", c => c.DateTime());
            AlterColumn("dbo.SendOrders", "ReceiveTime", c => c.DateTime());
            AlterColumn("dbo.Warehousings", "WarehousingTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Warehousings", "WarehousingTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SendOrders", "ReceiveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SendOrders", "SendTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MaterialOrders", "ReceiveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MaterialOrders", "SendTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.TeamAllocations", "CreateTime");
        }
    }
}
