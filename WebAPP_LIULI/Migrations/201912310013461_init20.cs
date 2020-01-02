namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BaseDatas", "HalfMoney", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BaseDatas", "HalfMoney");
        }
    }
}
