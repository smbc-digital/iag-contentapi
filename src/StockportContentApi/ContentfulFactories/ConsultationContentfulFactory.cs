using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ConsultationContentfulFactory : IContentfulFactory<ContentfulConsultation, Consultation>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsultationContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Consultation ToModel(ContentfulConsultation consultation)
        {
            return new Consultation(consultation.Title, consultation.ClosingDate, consultation.Link).StripData(_httpContextAccessor);
        }
    }
}