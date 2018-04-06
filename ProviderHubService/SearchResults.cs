using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    [DataContract]
    public class SearchResults
    {
        [DataMember]
        public List<FacilityProviderRelationship> FacilityProviderRelationships { get; set; }

        [DataMember]
        public List<Provider> Providers { get; set; }

        [DataMember]
        public List<Facility> Facilities { get; set; }

        [DataMember]
        public List<Vendor> Vendors { get; set; }

        public SearchResults()
        {
            this.FacilityProviderRelationships = new List<FacilityProviderRelationship>();
            this.Providers = new List<Provider>();
            this.Facilities = new List<Facility>();
            this.Vendors = new List<Vendor>();
        }
    }
}