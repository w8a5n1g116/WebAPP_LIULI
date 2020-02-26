namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init21 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WaterTeamAllocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        FixedAllocation = c.Double(nullable: false),
                        Allocation = c.Double(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Remarks = c.String(),
                        UserName = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WaterTeamAllocations", "User_Id", "dbo.Users");
            DropIndex("dbo.WaterTeamAllocations", new[] { "User_Id" });
            DropTable("dbo.WaterTeamAllocations");
        }
    }
}
