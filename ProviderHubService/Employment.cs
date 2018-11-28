using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ProviderHubService
{
    [DataContract]
    public class Employment
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string FacilityName { get; set; }
        [DataMember]
        public string SpecialtyName { get; set; }
        [DataMember]
        public int SequenceNumber { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public DateTime TerminationDate { get; set; }
    }
}