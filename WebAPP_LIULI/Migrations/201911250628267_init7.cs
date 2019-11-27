namespace WebAPP_LIULI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendOrders", "CustomerAddress", c => c.String());
            AddColumn("dbo.SendOrders", "Contact", c => c.String());
            AddColumn("dbo.SendOrders", "ContactPhone", c => c.String());
            AddColumn("dbo.TeamAllocations", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamAllocations", "Remarks");
            DropColumn("dbo.SendOrders", "ContactPhone");
            DropColumn("dbo.SendOrders", "Contact");
            DropColumn("dbo.SendOrders", "CustomerAddress");
        }
    }
}
