using System;
using Content.Server.Database._CD;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class CDRecordEntryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cd_record_entries",
                columns: table => new
                {
                    cd_record_entries_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    involved = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    foreign_creator = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cd_record_entries", x => x.cd_record_entries_id);
                    table.ForeignKey(
                        name: "FK_cd_record_entries_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cd_record_entries_cd_record_entries_id",
                table: "cd_record_entries",
                column: "cd_record_entries_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cd_record_entries_profile_id",
                table: "cd_record_entries",
                column: "profile_id");

            // Copy over entries to their table, entries will be erased from the profile the next time
            // the character is saved.
            //
            // NOTE: we loose data here if we migrate down, I don't think we intend to do that and
            //   we should be taking backups anyway before this migration
            migrationBuilder.Sql($"""
                INSERT INTO cd_record_entries (title, involved, description, type, profile_id)
                    SELECT
                        jsonb_array_elements.value ->> 'Title', jsonb_array_elements.value ->> 'Involved', jsonb_array_elements.value ->> 'Description',
                        {(int)CDModel.DbRecordEntryType.Medical}, profile_id
                    FROM
                        profile, jsonb_array_elements(cd_character_records -> 'MedicalEntries');
                """);

            migrationBuilder.Sql($"""
                INSERT INTO cd_record_entries (title, involved, description, type, profile_id)
                    SELECT
                        jsonb_array_elements.value ->> 'Title', jsonb_array_elements.value ->> 'Involved', jsonb_array_elements.value ->> 'Description',
                        {(int)CDModel.DbRecordEntryType.Security}, profile_id
                    FROM
                        profile, jsonb_array_elements(cd_character_records -> 'SecurityEntries')
                """);

            migrationBuilder.Sql($"""
                INSERT INTO cd_record_entries (title, involved, description, type, profile_id)
                    SELECT
                        jsonb_array_elements.value ->> 'Title', jsonb_array_elements.value ->> 'Involved', jsonb_array_elements.value ->> 'Description',
                        {(int)CDModel.DbRecordEntryType.Employment}, profile_id
                    FROM
                        profile, jsonb_array_elements(cd_character_records -> 'EmploymentEntries')
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cd_record_entries");
        }
    }
}
