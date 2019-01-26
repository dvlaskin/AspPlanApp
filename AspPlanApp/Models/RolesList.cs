namespace AspPlanApp.Models
{
    /// <summary>
    /// Перечень возможных ролей пользователя
    /// </summary>
    internal class AppRoles
    {
        /// <summary>
        /// Admin application role
        /// </summary>
        internal static string Admin => "admin";

        /// <summary>
        /// Owner business company role
        /// </summary>
        internal static string Owner => "owner";

        /// <summary>
        /// Employee of the business company role
        /// </summary>
        internal static string Staff => "staff";

        /// <summary>
        /// Simple user of the application, client of the business company role
        /// </summary>
        internal static string Client => "client";
    }
}