namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init18 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HalfQualityInspections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        ScrapCount = c.Double(nullable: false),
                        CheckTeam = c.String(),
                        CheckResult = c.String(),
                        UnqualifiedReson = c.String(),
                        Remarks = c.String(),
                        CheckUserName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.QualityInspections", "ProductName", c => c.String());
            AddColumn("dbo.Warehousings", "IsConfirm", c => c.Int(nullable: false));
            AddColumn("dbo.Warehousings", "ConfirmName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehousings", "ConfirmName");
            DropColumn("dbo.Warehousings", "IsConfirm");
            DropColumn("dbo.QualityInspections", "ProductName");
            DropTable("dbo.HalfQualityInspections");
        }
    }
}
