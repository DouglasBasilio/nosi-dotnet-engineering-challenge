using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class ContentSearcher
    {
        private readonly IDatabase<Content?, ContentDto> _database;

        public ContentSearcher(IDatabase<Content?, ContentDto> database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<IEnumerable<Content?>> SearchContents(string? title, string? genre)
        {
            IEnumerable<Content?> contents = await _database.ReadAll().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(genre))
            {
                return contents;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                contents = contents.Where(c => c?.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                contents = contents.Where(c => c?.GenreList?.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)) ?? false);
            }

            return contents;
        }
    }
}
