using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database;

public class ContentMapper : IMapper<Content, ContentDto>
{
    public Content Map(Guid id, ContentDto item)
    {
        return new Content(
            id,
            item.Title ?? throw new ArgumentNullException(nameof(item.Title)),
            item.SubTitle ?? throw new ArgumentNullException(nameof(item.SubTitle)),
            item.Description ?? throw new ArgumentNullException(nameof(item.Description)),
            item.ImageUrl ?? throw new ArgumentNullException(nameof(item.ImageUrl)),
            item.Duration ?? throw new ArgumentNullException(nameof(item.Duration)),
            item.StartTime ?? throw new ArgumentNullException(nameof(item.StartTime)),
            item.EndTime ?? throw new ArgumentNullException(nameof(item.EndTime)),
            item.GenreList);
    }

    public Content Patch(Content oldItem, ContentDto newItem)
    {
        // Cria uma nova lista para os gêneros atualizados
        List<string> updatedGenres = oldItem.GenreList.ToList();

        // Adiciona novos gêneros ao conteúdo atualizado
        foreach (var genre in newItem.GenreList)
        {
            if (!updatedGenres.Contains(genre))
            {
                updatedGenres.Add(genre);
            }
        }

        // Cria um novo objeto Content com os dados atualizados
        Content updatedItem = new Content(
            oldItem.Id,
            newItem.Title ?? oldItem.Title,
            newItem.SubTitle ?? oldItem.SubTitle,
            newItem.Description ?? oldItem.Description,
            newItem.ImageUrl ?? oldItem.ImageUrl,
            newItem.Duration ?? oldItem.Duration,
            newItem.StartTime ?? oldItem.StartTime,
            newItem.EndTime ?? oldItem.EndTime,
            updatedGenres // Usa a lista de gêneros atualizada
        );

        return updatedItem;
    }
}