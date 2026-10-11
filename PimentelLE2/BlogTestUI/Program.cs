using Microsoft.Extensions.Configuration;
using BlogDataLibrary.Database;
using BlogDataLibrary.Data;
using BlogDataLibrary.Models;

namespace BlogTestUI
{
    internal class Program
    {
        static SqlData GetConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();
            ISqlDataAccess dbAccess = new SqlDataAccess(config);
            SqlData db = new SqlData(dbAccess);
            return db;
        }

        static void Main(string[] args)
        {
            SqlData db = GetConnection();

            
            //Authenticate(db);
            //Register(db);
             AddPost(db);
            // ListPosts(db);
            // ShowPostDetails(db);

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        private static UserModel GetCurrentUser(SqlData db)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            return db.Authenticate(username, password);
        }

        public static void Authenticate(SqlData db)
        {
            UserModel user = GetCurrentUser(db);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials.");
            }
            else
            {
                Console.WriteLine($"Welcome, {user.UserName}");
            }
        }

        public static void Register(SqlData db)
        {
            Console.Write("Enter new username: ");
            var username = Console.ReadLine();
            Console.Write("Enter new password: ");
            var password = Console.ReadLine();
            Console.Write("Enter first name: ");
            var firstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine();

            db.Register(username, firstName, lastName, password);
        }

        private static void AddPost(SqlData db)
        {
            UserModel user = GetCurrentUser(db);
            if (user == null)
            {
                Console.WriteLine("Authentication failed. Cannot add post.");
                return;
            }

            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Write body: ");
            string body = Console.ReadLine();

            PostModel post = new PostModel
            {
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
                UserId = user.Id
            };

            db.AddPost(post);
        }

        private static void ListPosts(SqlData db)
        {
            List<ListPostModel> posts = db.ListPosts();
            foreach (ListPostModel post in posts)
            {
                string snippet = post.Body.Length > 20 ? post.Body.Substring(0, 20) : post.Body;
                Console.WriteLine($"{post.Id}. Title: {post.Title} by {post.UserName} [{post.DateCreated:yyyy-MM-dd}]");
                Console.WriteLine($"{snippet}...");
                Console.WriteLine();
            }
        }

        private static void ShowPostDetails(SqlData db)
        {
            Console.Write("Enter a post ID: ");
            int id = Int32.Parse(Console.ReadLine());

            ListPostModel post = db.ShowPostDetails(id);
            if (post != null)
            {
                Console.WriteLine(post.Title);
                Console.WriteLine($"by {post.FirstName} {post.LastName} [{post.UserName}]");
                Console.WriteLine();
                Console.WriteLine(post.Body);
                Console.WriteLine(post.DateCreated.ToString("MMM d yyyy"));
            }
            else
            {
                Console.WriteLine("Post not found.");
            }
        }
    }
}