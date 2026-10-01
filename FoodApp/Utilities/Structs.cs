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

    public enum KitchenRoles
    {
        /// <summary>
        /// Owner of the Kitchen, full controll
        /// </summary>
        owner,
        /// <summary>
        /// Administrator of the kitchen, allow to grant other roles
        /// </summary>
        admin,
        /// <summary>
        /// Editor, allow to change meals
        /// </summary>
        editor,
        /// <summary>
        /// Allow only for view of recipes
        /// </summary>
        inspector,
    }
}
