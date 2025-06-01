namespace Luftborn.API.Constants
{
    public static class Roles
    {
        public const string SuperAdmin = "Super Admin";
        public const string Writer = "Writer";
        public const string User = "User";

        public const string SuperAdminOrWriter = SuperAdmin + "," + Writer;
        public const string AllRoles = SuperAdmin + "," + Writer + "," + User;
    }
}
