namespace Repository.MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "RawSenderName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "RawSenderName");
        }
    }
}
