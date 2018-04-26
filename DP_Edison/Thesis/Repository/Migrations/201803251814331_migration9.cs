namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration9 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailRecipients", "EmailMessageId", "dbo.EmailMessages");
            DropForeignKey("dbo.EmailRecipients", "RecipientId", "dbo.Users");
            DropIndex("dbo.EmailRecipients", new[] { "RecipientId" });
            DropIndex("dbo.EmailRecipients", new[] { "EmailMessageId" });
            DropTable("dbo.EmailRecipients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EmailRecipients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientId = c.Int(),
                        EmailMessageId = c.Int(nullable: false),
                        RecipientType = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.EmailRecipients", "EmailMessageId");
            CreateIndex("dbo.EmailRecipients", "RecipientId");
            AddForeignKey("dbo.EmailRecipients", "RecipientId", "dbo.Users", "Id");
            AddForeignKey("dbo.EmailRecipients", "EmailMessageId", "dbo.EmailMessages", "Id", cascadeDelete: true);
        }
    }
}
