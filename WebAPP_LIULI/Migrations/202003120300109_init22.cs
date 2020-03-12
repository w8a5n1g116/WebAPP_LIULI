namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init22 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SendOrders", "Order_Id", "dbo.Orders");
            DropIndex("dbo.SendOrders", new[] { "Order_Id" });
            CreateTable(
                "dbo.MidSendOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.String(),
                        SendCount = c.Double(nullable: false),
                        ReceiveCount = c.Double(nullable: false),
                        SendDeterminePerson = c.String(),
                        ReceiveDeterminePerson = c.String(),
                        TaskStatus = c.String(),
                        Remarks = c.String(),
                        SendTime = c.DateTime(),
                        ReceiveTime = c.DateTime(),
                        CreateTime = c.DateTime(nullable: false),
                        CustomerAddress = c.String(),
                        Contact = c.String(),
                        ContactPhone = c.String(),
                        OneOfTonPrice = c.Double(nullable: false),
                        IsComfirm = c.Int(nullable: false),
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
            AddColumn("dbo.SendOrders", "MidSendOrder_Id", c => c.Int());
            CreateIndex("dbo.SendOrders", "MidSendOrder_Id");
            AddForeignKey("dbo.SendOrders", "MidSendOrder_Id", "dbo.MidSendOrders", "Id");
            DropColumn("dbo.SendOrders", "Order_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SendOrders", "Order_Id", c => c.Int());
            DropForeignKey("dbo.SendOrders", "MidSendOrder_Id", "dbo.MidSendOrders");
            DropForeignKey("dbo.MidSendOrders", "Order_Id", "dbo.Orders");
            DropIndex("dbo.SendOrders", new[] { "MidSendOrder_Id" });
            DropIndex("dbo.MidSendOrders", new[] { "Order_Id" });
            DropColumn("dbo.SendOrders", "MidSendOrder_Id");
            DropTable("dbo.MidSendOrders");
            CreateIndex("dbo.SendOrders", "Order_Id");
            AddForeignKey("dbo.SendOrders", "Order_Id", "dbo.Orders", "Id");
        }
    }
}
