using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.FAQs.Domain;

namespace Widgets.FAQs.Services
{
    public partial interface IFAQsService
    {
        Task DeleteFaq(FAQ faq);
        Task<IList<FAQ>> GetFaqs();

        Task<IList<FAQ>> GetFaqs(bool? onlyImportantQuestions);

        Task<FAQ> GetById(string faqId);

        Task InsertFaq(FAQ faq);
        Task UpdateFaq(FAQ faq);
    }
}
