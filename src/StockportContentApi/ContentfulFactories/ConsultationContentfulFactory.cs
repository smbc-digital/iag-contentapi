using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class ConsultationContentfulFactory : IContentfulFactory<ContentfulConsultation, Consultation>
    {
        public Consultation ToModel(ContentfulConsultation consultation)
        {
            return new Consultation(consultation.Title, consultation.ClosingDate, consultation.Link);
        }
    }
}