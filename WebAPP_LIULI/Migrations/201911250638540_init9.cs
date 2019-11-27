namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendOrders", "Order_Id", c => c.Int());
            CreateIndex("dbo.SendOrders", "Order_Id");
            AddForeignKey("dbo.SendOrders", "Order_Id", "dbo.Orders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SendOrders", "Order_Id", "dbo.Orders");
            DropIndex("dbo.SendOrders", new[] { "Order_Id" });
            DropColumn("dbo.SendOrders", "Order_Id");
        }
    }
}
