using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Text;

namespace ProviderHubService
{

    #region VENDOR

    [DataContract]
    public class Vendor
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int VendorFacilityMappingID { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public string NPI { get; set; }

        [DataMember]
        public string TaxID { get; set; }

        [DataMember]
        public string EPICVendorID { get; set; }

        [DataMember]
        public string ExternalID { get; set; }

        [DataMember]
        public string InternalNotes { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public DateTime LastUpdatedDate { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        [DataMember]
        public List<Address> AddressesList { get; set; }
        
        #region CONSTRUCTOR

        public Vendor()
        {
            this.AddressesList = new List<Address>();
        }

        #endregion

    }

    #endregion
}