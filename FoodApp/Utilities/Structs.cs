namespace FoodApp.Utilities
{
    public struct Sorting
    {
        public string fieldName;
        public SortDirection direction;
    }

    public enum SortDirection {
        ASC, DESC
    }

    public enum FormatMode
    {
        full,
        inspect
    }

    public enum Roles
    {
        user,
        admin
    }

    public enum UserStatus
    {
        active,
        inactive,
        timeout
    }
}
