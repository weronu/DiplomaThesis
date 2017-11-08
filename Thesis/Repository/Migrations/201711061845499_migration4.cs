namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration4 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Conversations", "EmailMessageId");
            AddForeignKey("dbo.Conversations", "EmailMessageId", "dbo.EmailMessages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Conversations", "EmailMessageId", "dbo.EmailMessages");
            DropIndex("dbo.Conversations", new[] { "EmailMessageId" });
        }
    }
}
