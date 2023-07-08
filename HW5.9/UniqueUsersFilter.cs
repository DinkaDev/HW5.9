using Microsoft.AspNetCore.Mvc.Filters;

namespace HW5._9
{
    public class UniqueUsersFilter : IActionFilter
    {
        private static readonly HashSet<string> UnigueUsers = new HashSet<string>();
        private static readonly object LockObject = new object();
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var uniqueUsersCount = 0;
            lock (LockObject)
            {
                uniqueUsersCount++;
            }

            WriteUniqueUsersToFile(uniqueUsersCount);
        }

        private void WriteUniqueUsersToFile(int uniqueUsersCount)
        {
            var filePath = "unique_users_count.txt";
            lock(LockObject)
            {
                using(var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"Unique users: {uniqueUsersCount}");
                }
            }
        }

        private object GetUserIdFromContext(HttpContext httpContext)
        {
            var user = httpContext.User.Identity.Name;
            return user;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = GetUserIdFromContext(context.HttpContext);
            if (!string.IsNullOrEmpty((string?)userId))
            {
                lock (LockObject)
                {
                    UnigueUsers.Add((string)userId);
                }
            }
        }
    }
}
