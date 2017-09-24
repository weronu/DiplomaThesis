namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageId = c.Guid(),
                        SenderId = c.Int(nullable: false),
                        InReplyToId = c.Guid(),
                        Subject = c.String(maxLength: 254, unicode: false),
                        Sent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.SenderId, cascadeDelete: false)
                .Index(t => t.SenderId);
            
            CreateTable(
                "dbo.EmailRecipients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EmailMessageId = c.Int(nullable: false),
                        RecipientType = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.EmailMessageId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 254, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailMessages", "SenderId", "dbo.Users");
            DropForeignKey("dbo.EmailRecipients", "UserId", "dbo.Users");
            DropForeignKey("dbo.EmailRecipients", "EmailMessageId", "dbo.EmailMessages");
            DropIndex("dbo.EmailRecipients", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailRecipients", new[] { "UserId" });
            DropIndex("dbo.EmailMessages", new[] { "SenderId" });
            DropTable("dbo.Users");
            DropTable("dbo.EmailRecipients");
            DropTable("dbo.EmailMessages");
        }
    }
}
