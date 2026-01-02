using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinniElectricidad.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class MigracionPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Usuarios_UsuarioIdUsuario",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "UsuarioIdUsuario",
                table: "Pagos",
                newName: "IdPresupuesto");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_UsuarioIdUsuario",
                table: "Pagos",
                newName: "IX_Pagos_IdPresupuesto");

            migrationBuilder.AlterColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraRealizado",
                table: "Pagos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PresupuestoId",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdUsuario",
                table: "Pagos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PresupuestoId",
                table: "Pagos",
                column: "PresupuestoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Presupuestos_IdPresupuesto",
                table: "Pagos",
                column: "IdPresupuesto",
                principalTable: "Presupuestos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Presupuestos_PresupuestoId",
                table: "Pagos",
                column: "PresupuestoId",
                principalTable: "Presupuestos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Usuarios_IdUsuario",
                table: "Pagos",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Presupuestos_IdPresupuesto",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Presupuestos_PresupuestoId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Usuarios_IdUsuario",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_IdUsuario",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_PresupuestoId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaHoraRealizado",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "PresupuestoId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "IdPresupuesto",
                table: "Pagos",
                newName: "UsuarioIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_IdPresupuesto",
                table: "Pagos",
                newName: "IX_Pagos_UsuarioIdUsuario");

            migrationBuilder.AlterColumn<double>(
                name: "Monto",
                table: "Pagos",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Usuarios_UsuarioIdUsuario",
                table: "Pagos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
