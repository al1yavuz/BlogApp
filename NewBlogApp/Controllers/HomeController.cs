using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewBlogApp.Models;

namespace NewBlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbConnection _connection;

        public HomeController(IDbConnection connection)
        {
            _connection = connection;
        }
        public IActionResult Index()
        {
            using var connection = new SqlConnection("Server=localhost;Database=blogprojesi;User Id=Ali Yavuz;Password=;Integrated Security=true;TrustServerCertificate=True");
            var posts = connection.Query<Post>("SELECT * FROM Posts ORDER BY CreatedDate DESC").ToList();

            return View(posts);
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
           
            var post = _connection.QuerySingleOrDefault<Post>("SELECT * FROM Posts WHERE Id = @id", new { id });
            var comments = _connection.Query<Comment>("SELECT * FROM Comments Where PostId = @id AND IsApproved = 1", new { id }).ToList();
            var model = new DetailViewModel { Post = post, Comments = comments };

            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(int id, Comment model)
        {
            
            var sql = @"INSERT INTO Comments
            (PostId, Name, Email, Body, IsApproved, CreatedDate)
            VALUES (@PostId, @Name, @Email, @Body, @IsApproved, @CreatedDate)";
            model.CreatedDate = DateTime.Now;
            model.IsApproved = false; // güvenlik amacýyla false yapýyoruz. aslýnda ön tanýmlý olarak false zaten
            model.PostId = id; // sayfa detay sayfasý olduðu için url de bulunan id, postId'yi ifade ediyor
            _connection.Execute(sql, model);

            ViewData["PostId"] = id;
            return View("AddCommentSuccess");

            //Redirect doðrudan istediðimiz adrese yönlendirme yapar
            //return Redirect($"/home/detail/{id}");
        }


    }
}
