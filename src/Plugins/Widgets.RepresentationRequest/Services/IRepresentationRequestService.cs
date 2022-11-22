using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;

namespace Widgets.RepresentationRequest.Services
{
    public partial interface IRepresentationRequestService
    {
        Task DeleteRequest(RepresentationRequestDomain request);
        Task<IList<RepresentationRequestDomain>> GetRequests();

        Task<RepresentationRequestDomain> GetById(string requestId);

        Task InsertRequest(RepresentationRequestDomain request);
        Task UpdateRequest(RepresentationRequestDomain request);
    }
}
