namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailMessages", "MessageId", c => c.String(maxLength: 70));
            AlterColumn("dbo.EmailMessages", "InReplyToId", c => c.String(maxLength: 70));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailMessages", "InReplyToId", c => c.String());
            AlterColumn("dbo.EmailMessages", "MessageId", c => c.String());
        }
    }
}
