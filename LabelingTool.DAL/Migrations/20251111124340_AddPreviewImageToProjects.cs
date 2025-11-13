using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabelingTool.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviewImageToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviewImage",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewImage",
                table: "Projects");
        }
    }
}
