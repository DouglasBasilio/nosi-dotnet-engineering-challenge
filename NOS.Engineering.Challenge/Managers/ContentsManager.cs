using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly ILogger<ContentsManager> _logger;

    public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger)
    {
        _database = database;
        _logger = logger;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        _logger.LogInformation("Getting many contents.");
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        _logger.LogInformation("Creating content.");
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        _logger.LogInformation("Getting content with id {Id}.", id);
        return _database.Read(id);
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
                content.GenreList.Append(genre);
            }
        }

        var contentDto = ConvertToContentDto(content);

        return await _database.Update(id, contentDto).ConfigureAwait(false);
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

        // Remove os gêneros especificados do conteúdo
        var updatedGenres = content.GenreList.Except(genres).ToList();

        // Cria uma cópia do conteúdo com os gêneros removidos
        var updatedContent = new ContentDto(
            content.Title,
            content.SubTitle,
            content.Description,
            content.ImageUrl,
            content.Duration,
            content.StartTime,
            content.EndTime,
            updatedGenres
        );

        return await _database.Update(id, updatedContent).ConfigureAwait(false);
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