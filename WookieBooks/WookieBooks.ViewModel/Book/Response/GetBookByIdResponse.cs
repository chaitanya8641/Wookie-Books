namespace WookieBooks.ViewModel.Book.Response
{
    public class GetBookByIdResponse
    {
        public BookModel? Data { get; set; }
    }

    public class BookModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string CoverImage { get; set; } = null!;
        public string Price { get; set; } = null!;
    }
}
