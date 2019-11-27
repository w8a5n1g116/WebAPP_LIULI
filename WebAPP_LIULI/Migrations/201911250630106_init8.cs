namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RewardPunishments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        Type = c.String(),
                        Team = c.String(),
                        Count = c.Double(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RewardPunishments");
        }
    }
}
