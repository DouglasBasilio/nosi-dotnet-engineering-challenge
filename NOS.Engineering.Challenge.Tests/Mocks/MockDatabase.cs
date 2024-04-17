using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Tests.Mocks
{
    public class MockDatabase : IDatabase<Content?, ContentDto>
    {
        private readonly IDictionary<Guid, Content> _mockData;

        public MockDatabase()
        {
            var mockDataGenerator = new MockData();
            _mockData = mockDataGenerator.GenerateMocks();
        }

        public Task<IEnumerable<Content?>> ReadAll()
        {
            return Task.FromResult<IEnumerable<Content?>>(_mockData.Values);
        }

        public Task<Content?> Read(Guid id)
        {
            if (_mockData.TryGetValue(id, out var content))
            {
                return Task.FromResult<Content?>(content);
            }
            return Task.FromResult<Content?>(null);
        }

        public Task<Content?> Create(ContentDto entity)
        {
            var content = new Content(
                Guid.NewGuid(),
                entity.Title,
                entity.SubTitle,
                entity.Description,
                entity.ImageUrl,
                entity.Duration ?? 0, // Se entity.Duration for nullable, forneça um valor padrão
                entity.StartTime ?? DateTime.MinValue, // Se entity.StartTime for nullable, forneça um valor padrão
                entity.EndTime ?? DateTime.MinValue, // Se entity.EndTime for nullable, forneça um valor padrão
                entity.GenreList ?? new List<string>() // Se entity.Genres for nullable, forneça uma lista vazia como padrão
            );
            _mockData.Add(content.Id, content);
            
            return Task.FromResult<Content?>(content);
        }

        public Task<Content?> Update(Guid id, ContentDto entity)
        {
            if (_mockData.TryGetValue(id, out var content))
            {
                // Update content properties
                content.Title = entity.Title;
                content.SubTitle = entity.SubTitle;
                content.Description = entity.Description;
                content.ImageUrl = entity.ImageUrl;
                content.Duration = entity.Duration ?? 0; // Se entity.Duration for nullable, forneça um valor padrão
                content.StartTime = entity.StartTime ?? DateTime.MinValue; // Se entity.StartTime for nullable, forneça um valor padrão
                content.EndTime = entity.EndTime ?? DateTime.MinValue; // Se entity.EndTime for nullable, forneça um valor padrão
                content.GenreList = entity.GenreList ?? new List<string>(); // Se entity.Genres for nullable, forneça uma lista vazia como padrão

                return Task.FromResult<Content?>(content);
            }
            return Task.FromResult<Content?>(null);
        }

        public Task<Guid> Delete(Guid id)
        {
            if (_mockData.ContainsKey(id))
            {
                _mockData.Remove(id);
                return Task.FromResult(id);
            }
            return Task.FromResult(Guid.Empty);
        }

        public IEnumerable<Content?> GetMockContents()
        {
            // Create mock content items
            var content1 = new Content(Guid.NewGuid(), "Title1", "Subtitle1", "Description1", "ImageUrl1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre1", "Genre2" });
            var content2 = new Content(Guid.NewGuid(), "Title2", "Subtitle2", "Description2", "ImageUrl2", 130, DateTime.Now, DateTime.Now.AddHours(3), new List<string> { "Genre3", "Genre4" });
            var content3 = new Content(Guid.NewGuid(), "Title3", "Subtitle3", "Description3", "ImageUrl3", 140, DateTime.Now, DateTime.Now.AddHours(4), new List<string> { "Genre5", "Genre6" });

            return new List<Content?> { content1, content2, content3 };
        }
    }
}
