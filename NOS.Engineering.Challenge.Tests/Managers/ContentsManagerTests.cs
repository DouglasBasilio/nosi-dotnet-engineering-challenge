using NOS.Engineering.Challenge.Managers;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Tests.Mocks;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;

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

        [Fact]
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

        [Fact]
        public async Task GetContent_By_Id()
        {
            ContentsManager contentsManager = new(_mockDatabase, _logger, _cache);
            
            var getId = await contentsManager.GetManyContents();
            var result = await contentsManager.GetContent(getId.FirstOrDefault().Id);

            Assert.NotNull(result);
        }
    }
}
