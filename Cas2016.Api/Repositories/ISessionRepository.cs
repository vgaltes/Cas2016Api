using System.Collections.Generic;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public interface ISessionRepository
    {
        List<SessionModel> GetAll();
        SessionModel Get(int sessionId);
    }
}