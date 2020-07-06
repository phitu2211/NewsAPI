namespace NewsAPI.Contracts.V1.Helper
{
    public static class Constant
    {
        public const string STATUS_ERROR = "Error";

        public const string STATUS_SUCESS = "Sucess";

        public const string NAME_NULL = "Name cannot be empty";

        public const string USER_NOT_EXIST = "User not exists";

        public const string USER_EXIST = "User with this email address already exists";

        public const string USER_PASS_WRONG = "User/password combination is wrong";

        public const string PASS_NULL = "Password is not null";

        public const string ROLE_ADMIN = "Admin";

        public const string ROLE_USER = "User";

        public static class ApiRoutes
        {
            private const string _root = "api";

            private const string _version = "v1";

            private const string _base = _root + "/" + _version;

            public static class Log
            {
                public const string GetLogByFilter = _base + "/logs";
                public const string GetLogById = _base + "/log/{logId}";
            }

            public static class Account
            {
                public const string Register = _base + "/register";
                public const string Login = _base + "/login";
                public const string UpdateAccount = _base + "/update/{userId}";
                public const string GetAccountById = _base + "/account/{userId}";
                public const string GetAccountByFilter = _base + "/accounts";
                public const string DeleteAccount = _base + "/account/{userId}";
                public const string GetAccountsByRoleId = _base + "/account/role/{roleId}";
            }

            public static class Menu
            {
                public const string CreateMenu = _base + "/menu";
                public const string GetMenus = _base + "/menus";
                public const string GetMenuById = _base + "/menu/{menuId}";
                public const string UpdateMenu = _base + "/menu/{menuId}";
                public const string DeleteMenu = _base + "/menu/{menuId}";
                public const string GetMenusByRoleName = _base + "/menu/role";
                public const string GetMenusByRoleId = _base + "/menu/role/{roleId}";
                public const string GetSubMenusById = _base + "/menu/{menuId}/sub";
            }

            public static class Role
            {
                public const string CreateRole = _base + "/role";
                public const string GetRoleByFilter = _base + "/roles";
                public const string GetRoleById = _base + "/role/{roleId}";
                public const string UpdateRole = _base + "/role/{roleId}";
                public const string UpdateRoleMenu = _base + "/role/menu/{roleId}";
                public const string UpdateRoleAccount = _base + "/role/account/{roleId}";
                public const string DeleteRole = _base + "/role/{roleId}";
            }

            public static class News
            {
                public const string GetNewsByFilter = _base + "/news";
                public const string GetNewsById = _base + "/news/{newsId}";
                public const string GetNewsByCategoryId = _base + "/news/category/{categoryId}";
                public const string CreateNews = _base + "/news";
                public const string UpdateNews = _base + "/news/{newsId}";
                public const string DeleteNews = _base + "/news/{newsId}";
            }

            public static class Category
            {
                public const string GetCategoryByFilter = _base + "/categories";
                public const string GetCategoryByNewsId = _base + "/categories/news/{newsId}";
                public const string GetCategoryById = _base + "/category/{categoryId}";
                public const string CreateCategory = _base + "/category";
                public const string UpdateCategory = _base + "/category/{categoryId}";
                public const string DeleteCategory = _base + "/category/{categoryId}";
            }
        }
    }
}
