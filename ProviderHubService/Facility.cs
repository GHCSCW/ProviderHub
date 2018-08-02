using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    [DataContract]
    public class Facility
    {
        #region PUBLIC PROPERTIES

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string FacilityName { get; set; }

        [DataMember]
        public string NPI { get; set; }

        [DataMember]
        public Address FacilityAddress { get; set; }

        [DataMember]
        public FacilityProviderRelationship FPRelationship { get; set; }

        [DataMember]
        public List<Specialty> FacilitySpecialties { get; set; }

        [DataMember]
        public List<Provider> FacilityProviders { get; set; }

        [DataMember]
        public string ExternalID { get; set; }

        [DataMember]
        public string InternalNotes { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedDate { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }
        
        #endregion

        #region CONSTRUCTOR

        public Facility()
        {
            this.FacilityAddress = new Address();
        }

        #endregion

    }
}