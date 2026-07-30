using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class ExceptionHandlerUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Transaction_Amount_Positive",
                table: "Transactions",
                sql: "Amount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TransactionLimit_CurrentSpent_NonNegative",
                table: "TransactionLimits",
                sql: "CurrentSpent >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TransactionLimit_LimitAmount_Positive",
                table: "TransactionLimits",
                sql: "LimitAmount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TransactionLimit_PeriodEnd_After_Start",
                table: "TransactionLimits",
                sql: "PeriodEnd > PeriodStart");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Goal_CurrentAmount_NonNegative",
                table: "Goals",
                sql: "CurrentAmount >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Goal_TargetAmount_Positive",
                table: "Goals",
                sql: "TargetAmount > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transaction_Amount_Positive",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TransactionLimit_CurrentSpent_NonNegative",
                table: "TransactionLimits");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TransactionLimit_LimitAmount_Positive",
                table: "TransactionLimits");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TransactionLimit_PeriodEnd_After_Start",
                table: "TransactionLimits");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Goal_CurrentAmount_NonNegative",
                table: "Goals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Goal_TargetAmount_Positive",
                table: "Goals");
        }
    }
}
