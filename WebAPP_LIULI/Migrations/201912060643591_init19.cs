namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HalfWarehousings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        InspectionCount = c.Double(nullable: false),
                        InspectionUserName = c.String(),
                        InspectionTime = c.DateTime(),
                        HalfWarehousingCount = c.Double(nullable: false),
                        HalfWarehousingName = c.String(),
                        HalfWarehousingTeam = c.String(),
                        HalfWarehousingTime = c.DateTime(),
                        Remarks = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        IsConfirm = c.Int(nullable: false),
                        ConfirmName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HalfWarehousings");
        }
    }
}
