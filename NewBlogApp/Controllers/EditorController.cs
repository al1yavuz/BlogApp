using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewBlogApp.Models;

namespace NewBlogApp.Controllers
{
    public class EditorController : Controller
    {
        private readonly IDbConnection _connection;

        public EditorController(IDbConnection connection)
        {
            _connection = connection;
        }
        public IActionResult Index()
        {
            
            var posts = _connection.Query<Post>("SELECT * FROM Posts ORDER BY CreatedDate DESC").ToList();

            return View(posts);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Post model)
        {
            model.CreatedDate = DateTime.Now;
            model.UpdateDate = DateTime.Now;
            
            _connection.Execute("INSERT INTO Posts (Title, CoverImageUrl, Summary, Content, CreatedDate, UpdateDate) VALUES (@Title, @CoverImageUrl, @Summary, @Content, @CreatedDate, @UpdateDate)", model);
            ViewData["SuccessMsg"] = "Yeni yazı eklendi.";
            return View();
        }

        public IActionResult Delete(int id)
        {
            
            _connection.Execute("DELETE FROM Posts WHERE Id = @id", new { id });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            
            var post = _connection.QuerySingleOrDefault<Post>("SELECT * FROM Posts WHERE Id = @id", new { id });
            return View("Add", post);
        }

        [HttpPost]
        public IActionResult Edit(Post model)
        {
            model.UpdateDate = DateTime.Now;
            
            var sql = @"UPDATE Posts 
            SET Title=@Title, Summary=@Summary, CoverImageUrl=@CoverImageUrl, Content=@Content, UpdateDate=@UpdateDate 
            WHERE Id = @Id";
            _connection.Execute(sql, model);

            return RedirectToAction("Index");
        }

        public IActionResult Comments()
        {
            
            var sql = @"SELECT Posts.Title as PostTitle, Comments.* FROM Comments 
            LEFT JOIN Posts ON Comments.PostId = Posts.Id 
            ORDER BY IsApproved, CreatedDate";
            var comments = _connection.Query<PostComment>(sql).ToList();
            return View(comments);
        }

        public IActionResult ApproveComment(int id)
        {
            
            _connection.Execute("UPDATE Comments SET IsApproved = 1, ApprovalDate = @ApprovalDate WHERE Id = @id", new
            {
                id,
                ApprovalDate = DateTime.Now
            });
            return RedirectToAction("Comments");
        }

        public IActionResult DeleteComment(int id)
        {
            
            _connection.Execute("DELETE FROM Comments WHERE Id = @id", new { id });
            return RedirectToAction("Comments");
        }
    }
}
