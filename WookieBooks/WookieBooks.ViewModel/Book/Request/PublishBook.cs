namespace WookieBooks.ViewModel.Book.Request
{
    public class PublishBook
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverImage { get; set; } = null!;
        public string Price { get; set; } = null!;
    }

    public class UpdatePublishedBook : PublishBook
    {
        public int BookId { get; set; }
    }
}
