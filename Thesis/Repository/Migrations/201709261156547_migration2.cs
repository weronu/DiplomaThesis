namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmailMessages", "Sent", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmailMessages", "Sent", c => c.DateTime(nullable: false));
        }
    }
}
