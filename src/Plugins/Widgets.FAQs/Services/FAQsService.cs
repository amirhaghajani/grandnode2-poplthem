using Grand.Business.Common.Interfaces.Security;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Grand.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.FAQs.Domain;

namespace Widgets.FAQs.Services
{
    public class FAQsService : IFAQsService
    {
        #region fields
        private readonly IRepository<FAQ> _reporistoryFAQ;
        private readonly IWorkContext _workContext;
        private readonly IAclService _aclService;
        private readonly ICacheBase _cacheManager;

        /// <summary>
        /// Key for sliders
        /// </summary>
        /// <remarks>
        /// {0} : Store id
        /// {1} : Slider type
        /// {2} : Object entry / categoryId || collectionId
        /// </remarks>
        public const string FAQS_MODEL_KEY = "Grand.faqs-{0}-{1}-{2}";
        public const string FAQS_PATTERN_KEY = "Grand.faqs";
        #endregion


        public FAQsService(IRepository<FAQ> reporistoryFaq,
            IWorkContext workContext, IAclService aclService,
            ICacheBase cacheManager)
        {
            this._reporistoryFAQ = reporistoryFaq;
            this._workContext = workContext;
            this._aclService = aclService;
            this._cacheManager = cacheManager;
        }
        public virtual async Task DeleteFaq(FAQ faq)
        {
            if (faq == null)
                throw new ArgumentNullException(nameof(faq));

            //clear cache
            await _cacheManager.RemoveByPrefix(FAQS_PATTERN_KEY);

            await _reporistoryFAQ.DeleteAsync(faq);
        }

        public virtual Task<FAQ> GetById(string faqId)
        {
            return _reporistoryFAQ.FirstOrDefaultAsync(x => x.Id == faqId);
        }

        public virtual async Task<IList<FAQ>> GetFaqs()
        {
            return await Task.FromResult(_reporistoryFAQ.Table
                .OrderByDescending(x => x.IsImportantQuestion)
                .ThenBy(x => x.FAQId)
                .ToList());
        }

        public async Task<IList<FAQ>> GetFaqs(bool? onlyImportantQuestions)
        {
            string cacheKey = string.Format(FAQS_MODEL_KEY, _workContext.CurrentStore.Id, 
                            onlyImportantQuestions.HasValue ? '-' : onlyImportantQuestions.ToString());

            return await _cacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from s in _reporistoryFAQ.Table
                            where onlyImportantQuestions.HasValue ? s.IsImportantQuestion==onlyImportantQuestions.Value : true
                            select s;

                var items = query.ToList();
                return await Task.FromResult(items.Where(c => _aclService.Authorize(c, _workContext.CurrentStore.Id)).ToList());
            });
        }

        public virtual async Task InsertFaq(FAQ faq)
        {
            if (faq == null)
                throw new ArgumentNullException(nameof(faq));

            //clear cache
            await _cacheManager.RemoveByPrefix(FAQS_PATTERN_KEY);

            await _reporistoryFAQ.InsertAsync(faq);
        }

        public virtual async Task UpdateFaq(FAQ faq)
        {
            if (faq == null)
                throw new ArgumentNullException(nameof(faq));

            //clear cache
            await _cacheManager.RemoveByPrefix(FAQS_PATTERN_KEY);

            await _reporistoryFAQ.UpdateAsync(faq);
        }
    }
}
