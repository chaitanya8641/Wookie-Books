using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WookieBooks.Entities;
using WookieBooks.Repository;
using WookieBooks.ViewModel.Book.Request;
using WookieBooks.ViewModel.Book.Response;

namespace WookieBooks.API.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getallpublishbooks")]
        public async Task<IActionResult> GetAllPublishBooks()
        {
            return Ok(await _unitOfWork.Books.GetAll());
        }

        [HttpGet]
        [Route("getBooksbytitle")]
        public async Task<IActionResult> GetBooksByTitle(string searchTitle)
        {
            var books = await _unitOfWork.Books.GetAll();
            if (!string.IsNullOrEmpty(searchTitle))
            {
                books = books.Where(book => book.Title == searchTitle).ToList();
            }

            return Ok(books);
        }


        [HttpGet]
        [Authorize]
        [Route("getbookbyid/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            GetBookByIdResponse response = new();
            var book = await _unitOfWork.Books.GetById(id);
            if (book != null)
            {
                response.Data = new BookModel
                {
                    BookId = book.BookId,
                    Author = book.Author,
                    Price = book.Price,
                    Description = book.Description,
                    Title = book.Title,
                    CoverImage = book.CoverImage,
                };
                return Ok(response);
            }

            return NotFound("Book not found");
        }

        [HttpGet]
        [Authorize]
        [Route("getbooksbyuserid/{id}")]
        public async Task<IActionResult> GetBooksByUserId(int id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                var books = await _unitOfWork.Books.Find(x => x.Author == user.Author_Pseudonym);
                return Ok(books);
            }

            return NotFound("User doesn't have any books");
        }

        [HttpPost]
        [Authorize]
        [Route("publishbook")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishBook([FromBody] PublishBook book)
        {
            var restrictedUsers = await _unitOfWork.RestrictedUsers.GetAll();
            if (book == null)
            {
                return BadRequest();
            }
            var user = await _unitOfWork.Users.GetById(Convert.ToInt32(User.Claims?.ToList()?.Find(x => x.Type == "UserId")?.Value));
            var userRestrict = restrictedUsers.Any(x => x.UserId == user.Id);
            if (userRestrict)
            {
                return BadRequest("Unable to publish your book on Wookie Books");
            }
            await _unitOfWork.Books.Add(new Book
            {
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                CoverImage = book.CoverImage,
                Author = user.Author_Pseudonym
            });
            var result = await _unitOfWork.Complete();


            if (result)
            {
                return Ok("Book Published Successfully!");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
        }



        [HttpPut]
        [Authorize]
        [Route("updatepublishedbook")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePublishedBook([FromBody] UpdatePublishedBook updateBook)
        {
            var user = await _unitOfWork.Users.GetById(Convert.ToInt32(User.Claims?.ToList()?.Find(x => x.Type == "UserId")?.Value));
            var books = await _unitOfWork.Books.Find(x => x.Author == user.Author_Pseudonym);
            var book = books.Where(x => x.BookId == updateBook.BookId).FirstOrDefault();
            if (book != null)
            {
                book.Title = updateBook.Title;
                book.Description = updateBook.Description;
                book.Price = updateBook.Price;
                book.CoverImage = updateBook.CoverImage;
                _unitOfWork.Books.Update(book);
                var result = await _unitOfWork.Complete();

                if (result)
                {
                    return Ok("Publish Book Updated Successfully!");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }

            return NotFound("Book not found.");
        }

        [HttpDelete]
        [Authorize]
        [Route("unpublishbook/{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnPublishBook(int id)
        {
            var user = await _unitOfWork.Users.GetById(Convert.ToInt32(User.Claims?.ToList()?.Find(x => x.Type == "UserId")?.Value));
            var books = await _unitOfWork.Books.Find(x => x.Author == user.Author_Pseudonym);
            var book = books.Where(x => x.BookId == id).FirstOrDefault();
            if (book == null)
            {
                return NotFound("Book Not Found");
            }
            _unitOfWork.Books.Remove(book);
            var result = await _unitOfWork.Complete();

            if (result)
            {
                return Ok("Book UnPublished Successfully");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
        }
    }
}
