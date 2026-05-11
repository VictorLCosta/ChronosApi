namespace CrossCutting.Models;

public interface IPagedQuery
{
    int? PageNumber { get; set; }

    int? PageSize { get; set; }

    string? Sort { get; set; }
}