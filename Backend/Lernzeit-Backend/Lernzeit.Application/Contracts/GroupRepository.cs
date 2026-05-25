using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface GroupRepository
{

    public Task<List<Group>> GetAllGroups();

}