namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QualityInspections", "CheckUserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.QualityInspections", "CheckUserName");
        }
    }
}
