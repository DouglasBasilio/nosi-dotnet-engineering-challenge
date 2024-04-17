using NOS.Engineering.Challenge.Managers;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Tests.Mocks;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Models;
using NuGet.Frameworks;

namespace NOS.Engineering.Challenge.Tests.Managers
{
    public class ContentsManagerTests
    {
        private readonly MockDatabase _mockDatabase;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ContentsManager> _logger;

        public ContentsManagerTests()
        {
            _mockDatabase = new MockDatabase();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _logger = new LoggerFactory().CreateLogger<ContentsManager>();
        }

        [Fact(DisplayName = "Search all")]
        public async Task SearchContents_ReturnsFilteredContents()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.SearchContents("", "");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(10, result.Count());
        }

        [Fact(DisplayName = "Search by title")]
        public async Task SearchContents_ReturnsFilteredContentsByTitle()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.SearchContents("Interstellar", "");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact(DisplayName = "Search by title - 404")]
        public async Task SearchContents_ReturnsFilteredContentsByTitle_404()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.SearchContents("The Simpsons", "");

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Search by genre")]
        public async Task SearchContents_ReturnsFilteredContentsByGenre()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.SearchContents("", "Romance");

            // Assert
            Assert.True(result.Count() >= 1);
        }

        [Fact(DisplayName = "Search by genre - 404")]
        public async Task SearchContents_ReturnsFilteredContentsByGenre_404()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.SearchContents("", "Religion");

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "GetManyContents - Obsolete")]
        public async Task GetManyContents_ReturnsContentsFromDatabase()
        {
            // Arrange
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            // Act
            var result = await contentsManager.GetManyContents();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(10, result.Count());
        }

        [Fact(DisplayName = "Get Content By Id")]
        public async Task GetContent_By_Id()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            var getId = await contentsManager.GetManyContents();
            var result = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Update Content")]
        public async Task UpdateContent()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            var getId = await contentsManager.SearchContents("", "");
            var content = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            var updatedContentDto = new Content
            {
                Title = "New Title",
                SubTitle = content.SubTitle,
                Description = content.Description,
                ImageUrl = content.ImageUrl,
                Duration = 115,
                StartTime = content.StartTime,
                EndTime = content.EndTime, 
                GenreList = content.GenreList 
            };

            var contentDTO = contentsManager.ConvertToContentDto(updatedContentDto);

            var result = await contentsManager.UpdateContent(content.Id, contentDTO);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal(115, result.Duration);
        }

        [Fact(DisplayName = "Delete Content")]
        public async Task DeleteContent()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            var getId = await contentsManager.SearchContents("", "");
            var content = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            var deletedId = await contentsManager.DeleteContent(content.Id);

            Assert.Equal(content.Id, deletedId);
        }

        [Fact(DisplayName = "Add Genres")]
        public async Task AddGenresToContent()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            var getId = await contentsManager.SearchContents("","");
            var content = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            var genres = new List<string> { "Adventure", "Sci-Fi" };

            var result = await contentsManager.AddGenresToContent(content.Id, genres);

            Assert.NotNull(result);
            Assert.True(result.GenreList.Count() >= 2);
        }

        [Fact(DisplayName = "Remove Genres")]
        public async Task RemoveGenresToContent()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);

            var getId = await contentsManager.SearchContents("", "");
            var content = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            var genreToRemove = content.GenreList.FirstOrDefault();

            var result = await contentsManager.RemoveGenresFromContent(content.Id, new List<string> { genreToRemove });

            Assert.True(genreToRemove.Count() != result.GenreList.Count());
        }
    }
}
