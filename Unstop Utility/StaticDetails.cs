namespace Unstop_Utility
{
    public static class StaticDetails
    {
        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE,
            PATCH
        }

        public static string SessionToken = "JWTToken";
        public static string SessionId = "sessionUserId";
        public static string SessionRole = "sessionUserRole";
        public static string SessionName = "sessionUserName";

        public static string AllJobsSP = "SELECT * FROM get_all_jobs(@user_id)";
        public static string MonthWiseApplicationsSP = "SELECT * FROM get_all_applications(@user_id,@month_value)";
    }
}
