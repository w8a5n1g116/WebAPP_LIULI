namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init16 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BaseDatas", "Name", c => c.Double(nullable: false));
            AlterColumn("dbo.BaseDatas", "Value", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BaseDatas", "Value", c => c.String());
            AlterColumn("dbo.BaseDatas", "Name", c => c.String());
        }
    }
}
