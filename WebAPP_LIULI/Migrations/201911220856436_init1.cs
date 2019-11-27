namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        OneOfWeight = c.Double(nullable: false),
                        OneOfPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamAllocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        FixedAllocation = c.Double(nullable: false),
                        Allocation = c.Double(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamAllocations", "User_Id", "dbo.Users");
            DropIndex("dbo.TeamAllocations", new[] { "User_Id" });
            DropTable("dbo.TeamAllocations");
            DropTable("dbo.Products");
        }
    }
}
