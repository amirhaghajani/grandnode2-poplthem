using Grand.Business.Common.Interfaces.Security;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Grand.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;

namespace Widgets.RepresentationRequest.Services
{

    public class RepresentationRequestService : IRepresentationRequestService
    {
        #region fields
        private readonly IRepository<RepresentationRequestDomain> _reporistoryRequest;
        private readonly IWorkContext _workContext;
        private readonly IAclService _aclService;
        private readonly ICacheBase _cacheManager;

        /// <summary>
        /// Key for sliders
        /// </summary>
        /// <remarks>
        /// {0} : Store id
        /// {1} : Is important
        /// </remarks>
        public const string REQUEST_MODEL_KEY = "Grand.representation-request-{0}-{1}";
        public const string REQUEST_PATTERN_KEY = "Grand.representation-request";
        #endregion


        public RepresentationRequestService(IRepository<RepresentationRequestDomain> reporistoryRequest,
            IWorkContext workContext, IAclService aclService,
            ICacheBase cacheManager)
        {
            this._reporistoryRequest = reporistoryRequest;
            this._workContext = workContext;
            this._aclService = aclService;
            this._cacheManager = cacheManager;
        }
        public virtual async Task DeleteRequest(RepresentationRequestDomain request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //clear cache
            await _cacheManager.RemoveByPrefix(REQUEST_PATTERN_KEY);

            await _reporistoryRequest.DeleteAsync(request);
        }

        public virtual Task<RepresentationRequestDomain> GetById(string requestId)
        {
            return _reporistoryRequest.FirstOrDefaultAsync(x => x.Id == requestId);
        }

        public virtual async Task<IList<RepresentationRequestDomain>> GetRequests()
        {
            return await Task.FromResult(_reporistoryRequest.Table
                .OrderByDescending(x => x.Id)
                .ToList());
        }

        

        public virtual async Task InsertRequest(RepresentationRequestDomain request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //clear cache
            await _cacheManager.RemoveByPrefix(REQUEST_PATTERN_KEY);

            await _reporistoryRequest.InsertAsync(request);
        }

        public virtual async Task UpdateRequest(RepresentationRequestDomain request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //clear cache
            await _cacheManager.RemoveByPrefix(REQUEST_PATTERN_KEY);

            await _reporistoryRequest.UpdateAsync(request);
        }
    }
}
