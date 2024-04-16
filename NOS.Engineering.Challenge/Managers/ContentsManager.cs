using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;

    public ContentsManager(IDatabase<Content?, ContentDto> database)
    {
        _database = database;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }

    public async Task<Content?> AddGenresToContent(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);

        if (content == null)
            return null;

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

    public async Task<Content?> RemoveGenresFromContent(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);

        if (content == null)
            return null;

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
}