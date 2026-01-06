using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class FixPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Pagos_Presupuestos_PresupuestoId')
        ALTER TABLE [Pagos] DROP CONSTRAINT [FK_Pagos_Presupuestos_PresupuestoId];

    IF EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Pagos_PresupuestoId'
          AND object_id = OBJECT_ID('dbo.Pagos')
    )
        DROP INDEX [IX_Pagos_PresupuestoId] ON [dbo].[Pagos];

    IF COL_LENGTH('dbo.Pagos', 'PresupuestoId') IS NOT NULL
        ALTER TABLE [dbo].[Pagos] DROP COLUMN [PresupuestoId];
    ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PresupuestoId",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PresupuestoId",
                table: "Pagos",
                column: "PresupuestoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Presupuestos_PresupuestoId",
                table: "Pagos",
                column: "PresupuestoId",
                principalTable: "Presupuestos",
                principalColumn: "Id");
        }
    }
}
