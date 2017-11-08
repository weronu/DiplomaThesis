namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Conversations", "EmailMessageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conversations", "EmailMessageId", c => c.Int(nullable: false));
        }
    }
}
