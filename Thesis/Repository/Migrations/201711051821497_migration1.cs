namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConversationId = c.Int(nullable: false),
                        EmailMessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageId = c.Guid(),
                        SenderId = c.Int(),
                        InReplyToId = c.Guid(),
                        Subject = c.String(),
                        Sent = c.DateTime(nullable: false),
                        XMLPosition = c.Int(nullable: false),
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.SenderId)
                .ForeignKey("dbo.Conversations", t => t.Conversation_Id)
                .Index(t => t.SenderId)
                .Index(t => t.Conversation_Id);
            
            CreateTable(
                "dbo.EmailRecipients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientId = c.Int(),
                        EmailMessageId = c.Int(nullable: false),
                        RecipientType = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailMessages", t => t.EmailMessageId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.RecipientId)
                .Index(t => t.RecipientId)
                .Index(t => t.EmailMessageId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailMessages", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.EmailMessages", "SenderId", "dbo.Users");
            DropForeignKey("dbo.EmailRecipients", "RecipientId", "dbo.Users");
            DropForeignKey("dbo.EmailRecipients", "EmailMessageId", "dbo.EmailMessages");
            DropIndex("dbo.EmailRecipients", new[] { "EmailMessageId" });
            DropIndex("dbo.EmailRecipients", new[] { "RecipientId" });
            DropIndex("dbo.EmailMessages", new[] { "Conversation_Id" });
            DropIndex("dbo.EmailMessages", new[] { "SenderId" });
            DropTable("dbo.Users");
            DropTable("dbo.EmailRecipients");
            DropTable("dbo.EmailMessages");
            DropTable("dbo.Conversations");
        }
    }
}
