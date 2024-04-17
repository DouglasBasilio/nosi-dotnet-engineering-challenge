using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public interface IContentsManager
{
    Task<IEnumerable<Content?>> GetManyContents();
    Task<Content?> CreateContent(ContentDto content);
    Task<Content?> GetContent(Guid id);
    Task<Content?> UpdateContent(Guid id, ContentDto content);
    Task<Guid> DeleteContent(Guid id);
    Task<Content?> AddGenresToContent(Guid id, IEnumerable<string> genres);
    Task<Content?> RemoveGenresFromContent(Guid id, IEnumerable<string> genres);
    Task<IEnumerable<Content?>> SearchContents(string title, string genre);
}