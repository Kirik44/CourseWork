using CourseWork.Models;
using System.Linq;

namespace CourseWork
{
    public class SampleData
    {
        public static void Initialize(WorkContext context)
        {
            if (!context.Moders.Any())
            {
                context.Moders.AddRange(
                    new Moder
                    {
                        Login = "Admin",
                        Password = "12345678",
                    });
                context.SaveChanges();
            }
        }
    }
}
