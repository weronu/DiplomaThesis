namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmailMessages", "Conversation_Id", "dbo.Conversations");
            DropIndex("dbo.EmailMessages", new[] { "Conversation_Id" });
            AddColumn("dbo.Conversations", "EmailMessageId", c => c.Int(nullable: false));
            DropColumn("dbo.EmailMessages", "Conversation_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailMessages", "Conversation_Id", c => c.Int());
            DropColumn("dbo.Conversations", "EmailMessageId");
            CreateIndex("dbo.EmailMessages", "Conversation_Id");
            AddForeignKey("dbo.EmailMessages", "Conversation_Id", "dbo.Conversations", "Id");
        }
    }
}
