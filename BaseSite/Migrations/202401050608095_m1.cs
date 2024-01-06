namespace BaseSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tb_CabinPanels", "Height", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_CabinPanels", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_CabinPanels", "Depth", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_DoorTopPanels", "Height", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_DoorTopPanels", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_DoorTopPanels", "Depth", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_HallPanels", "Height", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_HallPanels", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Tb_HallPanels", "Depth", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tb_HallPanels", "Depth");
            DropColumn("dbo.Tb_HallPanels", "Width");
            DropColumn("dbo.Tb_HallPanels", "Height");
            DropColumn("dbo.Tb_DoorTopPanels", "Depth");
            DropColumn("dbo.Tb_DoorTopPanels", "Width");
            DropColumn("dbo.Tb_DoorTopPanels", "Height");
            DropColumn("dbo.Tb_CabinPanels", "Depth");
            DropColumn("dbo.Tb_CabinPanels", "Width");
            DropColumn("dbo.Tb_CabinPanels", "Height");
        }
    }
}
