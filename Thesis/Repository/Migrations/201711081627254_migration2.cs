namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailMessages", "MessageId", c => c.String());
            AlterColumn("dbo.EmailMessages", "InReplyToId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailMessages", "InReplyToId", c => c.Guid());
            AlterColumn("dbo.EmailMessages", "MessageId", c => c.Guid());
        }
    }
}
