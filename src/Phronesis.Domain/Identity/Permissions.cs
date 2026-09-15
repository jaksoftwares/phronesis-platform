namespace Phronesis.Domain.Identity;

public static class Permissions
{
    public static class Users
    {
        public const string View = "users:view";
        public const string Create = "users:create";
        public const string Edit = "users:edit";
        public const string Delete = "users:delete";
        public const string AssignRoles = "users:assign_roles";
    }

    public static class Roles
    {
        public const string View = "roles:view";
        public const string Create = "roles:create";
        public const string Edit = "roles:edit";
        public const string Delete = "roles:delete";
    }

    public static class Teachers
    {
        public const string Approve = "teachers:approve";
        public const string ViewApplications = "teachers:view_applications";
    }

    public static class Content
    {
        public const string Create = "content:create";
        public const string Edit = "content:edit";
        public const string Delete = "content:delete";
        public const string Publish = "content:publish";
        public const string Review = "content:review";
    }

    public static class Classes
    {
        public const string Create = "classes:create";
        public const string Schedule = "classes:schedule";
        public const string Delete = "classes:delete";
    }
}
