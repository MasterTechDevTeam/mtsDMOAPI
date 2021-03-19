using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterTechDMO.API.Migrations
{
    public partial class Add_tbl_UserFriend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMOUserFriendList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    EmailId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRemoved = table.Column<bool>(nullable: false),
                    InsUser = table.Column<Guid>(nullable: false),
                    UpdUser = table.Column<Guid>(nullable: false),
                    InsDT = table.Column<DateTime>(nullable: false),
                    UpdDT = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMOUserFriendList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMOUserFriendList");
        }
    }
}
