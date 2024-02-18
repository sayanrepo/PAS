namespace BaseSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tb_CabinPanels", "Height", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_CabinPanels", "Width", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_CabinPanels", "Depth", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Height", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Width", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Depth", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_HallPanels", "Height", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_HallPanels", "Width", c => c.Double(nullable: false));
            AlterColumn("dbo.Tb_HallPanels", "Depth", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tb_HallPanels", "Depth", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_HallPanels", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_HallPanels", "Height", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Depth", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_DoorTopPanels", "Height", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_CabinPanels", "Depth", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_CabinPanels", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.Tb_CabinPanels", "Height", c => c.Int(nullable: false));
        }
    }
}
