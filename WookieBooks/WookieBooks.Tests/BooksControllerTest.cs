using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WookieBooks.Entities;
using WookieBooks.Tests.Responses;
using WookieBooks.ViewModel.Authentication.Request;
using WookieBooks.ViewModel.Book.Request;
using WookieBooks.ViewModel.Book.Response;

namespace WookieBooks.Tests
{
    public class BooksControllerTest : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;

        public BooksControllerTest(TestingWebAppFactory<Program> factory)
        {
            var appFactory = new WebApplicationFactory<Program>();
            _client = appFactory.CreateClient();
        }

        [Fact]
        public async Task GetAllBooks_WhenCalled_ReturnsAllBooks()
        {
            var response = await _client.GetAsync("/api/book/getallpublishbooks");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<List<Book>>(responseString);
            parsedResponse = parsedResponse.Distinct().ToList();
            Assert.NotNull(parsedResponse);
        }


        [Fact]
        public async Task GetBookBySearchBookTitle_WhenCalled_ReturnsBook()
        {
            var response = await _client.GetAsync("/api/book/getBooksbytitle?searchTitle=Test12");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<List<Book>>(responseString);
            parsedResponse = parsedResponse.Distinct().ToList();
            Assert.NotNull(parsedResponse);
            Assert.Equal("HG", parsedResponse[0].Author);
        }

        [Fact]
        public async Task GetBookById_WhenCalled_ReturnsBook()
        {
            await PerformLogin("HGibbs", "password");
            var response = await _client.GetAsync("/api/book/getbookbyid/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<GetBookByIdResponse>(responseString);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(parsedResponse);
            Assert.Equal("HG", parsedResponse.Data?.Author.ToString());
        }

        [Fact]
        public async Task GetBookById_WhenCalled_ReturnsNotFound()
        {
            await PerformLogin("HGibbs", "password");
            var response = await _client.GetAsync("/api/book/getbooksbyuserid/10");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal("User doesn't have any books", responseString);
        }


        [Fact]
        public async Task GetBookByUserId_WhenCalled_ReturnsALLBooks()
        {
            await PerformLogin("HGibbs", "password");
            var response = await _client.GetAsync("/api/book/getbooksbyuserid/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<List<Book>>(responseString);
            parsedResponse = parsedResponse.Distinct().ToList();
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(parsedResponse);
        }


        [Fact]
        public async Task PublishBook_WhenCalledWithValidUser_ReturnsSuccess()
        {
            await PerformLogin("HGibbs", "password");
            var book = new PublishBook { Title = "Demo", Description = "Demo", CoverImage = "Demo", Price = "1" };
            var response = await _client.PostAsJsonAsync<PublishBook>("/api/book/publishbook", book);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PublishBook_WhenCalledWithInvalidUser_ReturnsFailure()
        {
            await PerformLogin("Anakin24", "password");
            var book = new PublishBook { Title = "Demo", Description = "Demo", CoverImage = "Demo", Price = "1" };
            var response = await _client.PostAsJsonAsync<PublishBook>("/api/book/publishbook", book);
            Assert.Equal("Bad Request", response.ReasonPhrase?.ToString());
        }


        [Fact]
        public async Task UpdatePublishedBook_WhenCalled_ReturnsSuccess()
        {
            await PerformLogin("HGibbs", "password");
            var book = new UpdatePublishedBook { BookId = 1, Title = "Demo", Description = "Demo1", CoverImage = "Demo", Price = "1" };
            var response = await _client.PutAsJsonAsync<UpdatePublishedBook>("/api/book/updatepublishedbook", book);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdatePublishedBook_WhenCalledNoBookData_ReturnsFailure()
        {
            await PerformLogin("HGibbs", "password");
            var book = new UpdatePublishedBook { BookId = 10, Title = "Demo", Description = "Demo1", CoverImage = "Demo", Price = "1" };
            var response = await _client.PutAsJsonAsync<UpdatePublishedBook>("/api/book/updatepublishedbook", book);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Book not found.", responseString);
        }

        [Fact]
        public async Task UpdatePublishedBook_WhenCalledNUll_ReturnsFailure()
        {
            await PerformLogin("HGibbs", "password");
            var book = new UpdatePublishedBook();
            var response = await _client.PutAsJsonAsync<UpdatePublishedBook>("/api/book/updatepublishedbook", book);
            Assert.Equal("Bad Request", response.ReasonPhrase);
        }

        [Fact]
        public async Task UnPublishBook_WhenCalledWithValidUser_ReturnsSuccess()
        {
            await PerformLogin("HGibbs", "password");
            var response = await _client.DeleteAsync("/api/book/unpublishbook/1");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Book UnPublished Successfully", responseString);
        }

        [Fact]
        public async Task UnPublishBook_WhenCalledWithInvalidUser_ReturnsUnSuccessfull()
        {
            await PerformLogin("Anakin24", "password");
            var response = await _client.DeleteAsync("/api/book/unpublishbook/1");
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal("Book Not Found", responseString);
        }

        private async Task PerformLogin(string userName, string password)
        {
            var user = new AuthenticationRequest
            {
                Username = userName,
                Password = password
            };

            var res = await _client.PostAsJsonAsync("/api/auth/authenticate", user);
            var responseString = await res.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<TestTokenResponse>(responseString);
            AddAuthToken(parsedResponse.Token);
        }

        private void AddAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

    }
}
