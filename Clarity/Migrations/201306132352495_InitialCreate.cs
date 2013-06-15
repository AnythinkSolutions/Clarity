namespace Clarity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsFixed = c.Boolean(nullable: false),
                        Frequency = c.Time(nullable: false),
                        LastPayment = c.DateTime(nullable: false),
                        PaymentDay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPaid = c.Boolean(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bills", t => t.BillId, cascadeDelete: true)
                .Index(t => t.BillId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Payments", new[] { "BillId" });
            DropForeignKey("dbo.Payments", "BillId", "dbo.Bills");
            DropTable("dbo.Payments");
            DropTable("dbo.Bills");
        }
    }
}
