using Common.Models;

namespace JWTService
{
    /// <summary>
    /// 虚拟数据，模拟从数据库中读取用户
    /// </summary>
    public static class TemporaryData
    {
        private static List<User> Users = new List<User>() { new User { Id = "001", UserName = "张三", Password = "111111" }, new User { Id = "002", UserName = "李四", Password = "222222" } };

        public static User GetUser(string code)
        {
            return Users.FirstOrDefault(m => m.Id.Equals(code));
        }
    }
}
