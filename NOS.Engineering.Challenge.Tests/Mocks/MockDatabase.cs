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
                entity.Duration ?? 0,
                entity.StartTime ?? DateTime.MinValue,
                entity.EndTime ?? DateTime.MinValue,
                entity.GenreList ?? new List<string>()
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
                content.Duration = entity.Duration ?? 0;
                content.StartTime = entity.StartTime ?? DateTime.MinValue;
                content.EndTime = entity.EndTime ?? DateTime.MinValue;
                content.GenreList = entity.GenreList ?? new List<string>();

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
    }
}
