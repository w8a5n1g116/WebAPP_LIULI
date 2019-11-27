namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CustomerCompanyNumber = c.String(),
                        CustomerAddress = c.String(),
                        CustomerPhone = c.String(),
                        Contact = c.String(),
                        ContactPhone = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeliveryTime = c.DateTime(nullable: false),
                        ProductName = c.String(),
                        ProductCount = c.Double(nullable: false),
                        OrderStatus = c.String(),
                        Salesman = c.String(),
                        Remarks = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CarNumber = c.String(),
                        PhoneNumber = c.String(),
                        CarType = c.String(),
                        MaxLoad = c.String(),
                        PersonId = c.String(),
                        BankNumer = c.String(),
                        BankName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MaterialOrders",
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
                        SendTime = c.DateTime(nullable: false),
                        ReceiveTime = c.DateTime(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        MaterialDriver_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Drivers", t => t.MaterialDriver_Id)
                .Index(t => t.MaterialDriver_Id);
            
            CreateTable(
                "dbo.SendOrders",
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
                        SendTime = c.DateTime(nullable: false),
                        ReceiveTime = c.DateTime(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        MaterialDriver_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Drivers", t => t.MaterialDriver_Id)
                .Index(t => t.MaterialDriver_Id);
            
            CreateTable(
                "dbo.QualityInspections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        ScrapCount = c.Double(nullable: false),
                        CheckTeam = c.String(),
                        CheckResult = c.String(),
                        UnqualifiedReson = c.String(),
                        Remarks = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UserType = c.String(),
                        UserPermission = c.String(),
                        UserTeam = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Warehousings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        WarehousingCount = c.Double(nullable: false),
                        WarehousingTeam = c.String(),
                        WarehousingTime = c.DateTime(nullable: false),
                        Remarks = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SendOrders", "MaterialDriver_Id", "dbo.Drivers");
            DropForeignKey("dbo.MaterialOrders", "MaterialDriver_Id", "dbo.Drivers");
            DropForeignKey("dbo.Orders", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.SendOrders", new[] { "MaterialDriver_Id" });
            DropIndex("dbo.MaterialOrders", new[] { "MaterialDriver_Id" });
            DropIndex("dbo.Orders", new[] { "Customer_Id" });
            DropTable("dbo.Warehousings");
            DropTable("dbo.Users");
            DropTable("dbo.QualityInspections");
            DropTable("dbo.SendOrders");
            DropTable("dbo.MaterialOrders");
            DropTable("dbo.Drivers");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
        }
    }
}
