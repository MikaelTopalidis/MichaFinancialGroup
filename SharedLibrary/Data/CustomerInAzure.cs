using Azure.Search.Documents.Indexes;

namespace SharedLibrary.Data
{
    public class CustomerInAzure
    {
        [SimpleField(IsKey = true, IsFilterable = true, IsSortable = true)]
        public string Id { get; set; }

        [SearchableField(IsSortable = true)]
        public string Name { get; set; }

        [SearchableField(IsSortable = true)]
        public string City { get; set; }

        [SimpleField(IsSortable = true)]
        public string Address { get; set; }

        [SimpleField(IsSortable = true)]
        public string PersonalNumber { get; set; }

    }
}
