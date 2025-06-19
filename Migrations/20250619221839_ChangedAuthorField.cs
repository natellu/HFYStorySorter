using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HFYStorySorter.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAuthorField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Posts",
                newName: "Author");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Posts",
                newName: "AuthorName");
        }
    }
}
