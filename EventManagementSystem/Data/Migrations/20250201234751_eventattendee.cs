using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class eventattendee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendee_Attendees_AttendeeId",
                table: "EventAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendee_Events_EventId",
                table: "EventAttendee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendee",
                table: "EventAttendee");

            migrationBuilder.RenameTable(
                name: "EventAttendee",
                newName: "EventAttendees");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendee_EventId",
                table: "EventAttendees",
                newName: "IX_EventAttendees_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendee_AttendeeId",
                table: "EventAttendees",
                newName: "IX_EventAttendees_AttendeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees",
                column: "Event_AttendeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_Attendees_AttendeeId",
                table: "EventAttendees",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "AttendeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_Events_EventId",
                table: "EventAttendees",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_Attendees_AttendeeId",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_Events_EventId",
                table: "EventAttendees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees");

            migrationBuilder.RenameTable(
                name: "EventAttendees",
                newName: "EventAttendee");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendees_EventId",
                table: "EventAttendee",
                newName: "IX_EventAttendee_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendees_AttendeeId",
                table: "EventAttendee",
                newName: "IX_EventAttendee_AttendeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendee",
                table: "EventAttendee",
                column: "Event_AttendeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendee_Attendees_AttendeeId",
                table: "EventAttendee",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "AttendeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendee_Events_EventId",
                table: "EventAttendee",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
