using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create procedure uspReportSales(@DateStart Date, @DateEnd DATE)
			as
			begin
			
			select 
			C.Title ConcertName,
			SUM(S.Total) as Total

			from Sale s (nolock)
			inner join Concert c (nolock) on s.ConcertId = c.Id
			and c.Status = 1
			and Cast(S.SaleDate as date) between @DateStart and @DateEnd
			group by C.Title
			order by 2 desc
			end;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE uspReportSales");
        }
    }
}
