namespace Clarity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removelastpayment : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Bills", "LastPayment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bills", "LastPayment", c => c.DateTime(nullable: false));
        }
    }
}
