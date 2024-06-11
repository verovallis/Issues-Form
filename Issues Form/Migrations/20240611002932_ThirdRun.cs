using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssuesForm.Migrations
{
    /// <inheritdoc />
    public partial class ThirdRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CCEmail",
                table: "Form",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCEmail",
                table: "Form");
        }
    }
}
