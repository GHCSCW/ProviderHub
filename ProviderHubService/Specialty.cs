using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    [DataContract]
    public class Specialty
    {
        #region PUBLIC PROPERTIES      

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int MappingID { get; set; }

        [DataMember]
        public int SequenceNumber { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        //for PH
        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public DateTime TerminationDate { get; set; }

        [DataMember]
        public string SpecialtyType { get; set; }

        [DataMember]
        public int ParentSpecialtyID { get; set; }
        //end for PH

        #endregion
    }
}