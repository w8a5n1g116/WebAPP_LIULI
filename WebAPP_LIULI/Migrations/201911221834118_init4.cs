namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TaskId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "TaskId");
        }
    }
}
