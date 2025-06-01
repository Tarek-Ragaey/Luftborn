using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luftborn.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                            -- 1. Insert Roles
                            DECLARE @SuperAdminRoleId UNIQUEIDENTIFIER = NEWID();
                            DECLARE @WriterRoleId UNIQUEIDENTIFIER = NEWID();
                            DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();

                            INSERT INTO AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
                            VALUES
                            (@SuperAdminRoleId, 'Super Admin', 'SUPER ADMIN', NEWID()),
                            (@WriterRoleId, 'Writer', 'WRITER', NEWID()),
                            (@UserRoleId, 'User', 'USER', NEWID());

                            -- 2. Insert Role Translations
                            INSERT INTO RoleTranslations(RoleId, LanguageKey, TranslatedName)
                            VALUES
                            -- Super Admin translations
                            (@SuperAdminRoleId, 'en', 'Super Admin'),
                            (@SuperAdminRoleId, 'ar', N'مشرف عام'),

                            -- Writer translations
                            (@WriterRoleId, 'en', 'Writer'),
                            (@WriterRoleId, 'ar', N'كاتب'),

                            -- User translations
                            (@UserRoleId, 'en', 'User'),
                            (@UserRoleId, 'ar', N'مستخدم');

                            -- 3. Insert Super Admin User
                            DECLARE @SuperAdminUserId UNIQUEIDENTIFIER = NEWID();
                            DECLARE @PasswordHash NVARCHAR(MAX) = N'AQAAAAEAACcQAAAAENnz5dlZo4Eue96u5/GCa2qakq7x9UzTzHRPj+GCVjlNFvDBBBwc2uWLYUN6/G8i8Q==';
                            DECLARE @SecurityStamp UNIQUEIDENTIFIER = NEWID();
                            DECLARE @ConcurrencyStamp UNIQUEIDENTIFIER = NEWID();

                            INSERT INTO AspNetUsers(
                                Id,
                                UserName,
                                NormalizedUserName,
                                Email,
                                NormalizedEmail,
                                EmailConfirmed,
                                PasswordHash,
                                SecurityStamp,
                                ConcurrencyStamp,
                                PhoneNumberConfirmed,
                                TwoFactorEnabled,
                                LockoutEnabled,
                                AccessFailedCount
                            )
                            VALUES(
                                @SuperAdminUserId,
                                'admin@admin.com',
                                'ADMIN@ADMIN.COM',
                                'admin@admin.com',
                                'ADMIN@ADMIN.COM',
                                1, -- EmailConfirmed
                                @PasswordHash,
                                @SecurityStamp,
                                @ConcurrencyStamp,
                                0, -- PhoneNumberConfirmed
                                0, -- TwoFactorEnabled
                                1, -- LockoutEnabled
                                0  -- AccessFailedCount
                            );

                            -- 4. Assign Super Admin role to the Super Admin user
                            INSERT INTO AspNetUserRoles(UserId, RoleId)
                            VALUES(@SuperAdminUserId, @SuperAdminRoleId);

                            -- 5. Insert Writer User
                            DECLARE @WriterUserId UNIQUEIDENTIFIER = NEWID();
                            DECLARE @WriterSecurityStamp UNIQUEIDENTIFIER = NEWID();
                            DECLARE @WriterConcurrencyStamp UNIQUEIDENTIFIER = NEWID();

                            INSERT INTO AspNetUsers(
                                Id,
                                UserName,
                                NormalizedUserName,
                                Email,
                                NormalizedEmail,
                                EmailConfirmed,
                                PasswordHash,
                                SecurityStamp,
                                ConcurrencyStamp,
                                PhoneNumberConfirmed,
                                TwoFactorEnabled,
                                LockoutEnabled,
                                AccessFailedCount
                            )
                            VALUES(
                                @WriterUserId,
                                'writer@writer.com',
                                'WRITER@WRITER.COM',
                                'writer@writer.com',
                                'WRITER@WRITER.COM',
                                1, -- EmailConfirmed
                                @PasswordHash,
                                @WriterSecurityStamp,
                                @WriterConcurrencyStamp,
                                0, -- PhoneNumberConfirmed
                                0, -- TwoFactorEnabled
                                1, -- LockoutEnabled
                                0  -- AccessFailedCount
                            );

                            -- 6. Assign Writer role to the Writer user
                            INSERT INTO AspNetUserRoles(UserId, RoleId)
                            VALUES(@WriterUserId, @WriterRoleId);


            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
