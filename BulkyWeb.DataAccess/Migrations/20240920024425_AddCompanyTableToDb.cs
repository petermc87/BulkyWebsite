using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Companies",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "string"),
            //        StreetAddress = table.Column<string>(type: "string"),
            //        City = table.Column<string>(type: "string"),
            //        State = table.Column<string>(type: "string"),
            //        PostalCode = table.Column<string>(type: "string")
            //    });

            //migrationBuilder.InsertData(
            //    table: "Companies",
            //    columns: new[] { "Id", "Name", "StreetAddress", "City", "State", "PostalCode" },
            //    values: new object[,]
            //    {
            //        { 1, "Navillus", "633 3rd avenue", "New York", "NY", "10017" },
            //        { 2, "Peters", "300 40th street", "Queens", "NY", "11101" },
            //        { 3, "Best Sandwich", "333 3rd street", "Brooklyn", "NY", "24687" }
            //    });
        }

        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
