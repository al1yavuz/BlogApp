namespace NewBlogApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

    public class PostComment : Comment
    {
        public string PostTitle { get; set; }
    }
}
