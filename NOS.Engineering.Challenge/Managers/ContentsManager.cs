using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly ILogger<ContentsManager> _logger;
    private readonly IMemoryCache _cache;

    public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger, IMemoryCache cache)
    {
        _database = database;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<Content?>> GetManyContents()
    {
        const string cacheKey = "AllContents";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Content?> contents))
        {
            _logger.LogInformation("Getting many contents.");
            contents = await _database.ReadAll().ConfigureAwait(false);
            _cache.Set(cacheKey, contents, TimeSpan.FromMinutes(5));
        }
        return contents;
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        _logger.LogInformation("Creating content.");
        return _database.Create(content);
    }

    public async Task<Content?> GetContent(Guid id)
    {
        _logger.LogInformation("Getting content with id {Id}.", id);

        if (_cache.TryGetValue(id, out Content? cachedContent))
        {
            return cachedContent;
        }

        var content = await _database.Read(id).ConfigureAwait(false);

        if (content != null)
        {
            _cache.Set(id, content, TimeSpan.FromMinutes(1));
        }

        return content;
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        _logger.LogInformation("Updating content with id {Id}.", id);
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        _logger.LogInformation("Deleting content with id {Id}.", id);
        return _database.Delete(id);
    }

    public async Task<Content?> AddGenresToContent(Guid id, IEnumerable<string> genres)
    {
        _logger.LogInformation("Adding genres to content with id {Id}.", id);

        var content = await _database.Read(id).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogWarning("Content with id {Id} not found.", id);
            return null;
        }

        // Adiciona apenas os gêneros que não estão presentes no conteúdo
        foreach (var genre in genres)
        {
            if (!content.GenreList.Contains(genre))
            {
                content.GenreList = content.GenreList.Append(genre).ToList();
            }
        }

        var contentDto = ConvertToContentDto(content);
        var updatedContent = await _database.Update(id, contentDto).ConfigureAwait(false);

        return updatedContent;
    }

    public async Task<Content?> RemoveGenresFromContent(Guid id, IEnumerable<string> genres)
    {
        _logger.LogInformation("Removing genres from content with id {Id}.", id);

        var content = await _database.Read(id).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogWarning("Content with id {Id} not found.", id);
            return null;
        }

        // Removo os gêneros especificados do conteúdo
        content.GenreList = content.GenreList.Except(genres).ToList();

        var contentDto = ConvertToContentDto(content);
        var updatedContent = await _database.Update(id, contentDto).ConfigureAwait(false);

        return updatedContent;
    }

    private ContentDto ConvertToContentDto(Content content)
    {
        return new ContentDto(
            content.Title,
            content.SubTitle,
            content.Description,
            content.ImageUrl,
            content.Duration,
            content.StartTime,
            content.EndTime,
            content.GenreList
        );
    }
}