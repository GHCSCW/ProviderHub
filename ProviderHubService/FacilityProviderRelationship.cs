﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    [DataContract]
    public class FacilityProviderRelationship
    {

        #region PRIVATE VARIABLES

        private bool relationshipStatus;

        #endregion

        #region PUBLIC PROPERTIES

        [DataMember]
        public int RelationshipID { get; set; }

        [DataMember]
        public Facility Facility { get; set; }

        [DataMember]
        public Provider Provider { get; set; }

        [DataMember]
        public Vendor Vendor { get; set; }

        [DataMember]
        public bool? ExternalProviderIndicator { get; set; }

        [DataMember]
        public bool? AcceptingNewPatientIndicator { get; set; }

        [DataMember]
        public bool? PrescriberIndicator { get; set; }

        [DataMember]
        public bool? ReferralIndicator { get; set; }

        [DataMember]
        public bool? PCPEligibleIndicator { get; set; }

        [DataMember]
        public bool? FloatProviderIndicator { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public DateTime TerminationDate { get; set; }

        [DataMember]
        public int SequenceNumber { get; set; }

        [DataMember]
        public string ProviderEmail { get; set; }

        [DataMember]
        public string ProviderPhoneNumber { get; set; }

        [DataMember]
        public string ProviderExtensionNumber { get; set; }

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

        [DataMember]
        public List<BehavioralHealthAttribute> BehavioralHealthAttributes { get; set; }

        [DataMember]
        private bool RelationshipStatus
        {
            get { return (DateTime.Today.Date >= this.EffectiveDate && DateTime.Today.Date <= this.TerminationDate); }
            set { relationshipStatus = value; }
        }
        
        //[DataMember]
        //public List<Specialty> FacilityProviderSpecialties { get; set; }

        //[DataMember]
        //public List<ProviderFacilityAvailability> AvailabilityList { get; set; }

        #endregion

        #region CONSTRUCTOR

        public FacilityProviderRelationship()
        {
            this.Facility = new Facility();
            this.Provider = new Provider();
            this.Vendor = new Vendor();
            this.BehavioralHealthAttributes = new List<BehavioralHealthAttribute>();
            //this.FacilityProviderSpecialties = new List<Specialty>();
            //this.AvailabilityList = new List<ProviderFacilityAvailability>();
        }
        public Dictionary<string, bool?> getshortObj(DataRow x)
        {
            Dictionary<string, bool?> toReturn = new Dictionary<string, bool?>();
            toReturn.Add("ExternalProviderIndicator", x.Field<bool?>("EXTERNAL_PROVIDER_INDICATOR"));
            toReturn.Add("AcceptingNewPatientIndicator", x.Field<bool?>("ACCEPTING_NEW_PATIENT_INDICATOR"));
            toReturn.Add("PrescriberIndicator", x.Field<bool?>("PRESCRIBER_INDICATOR"));
            toReturn.Add("ReferralIndicator", x.Field<bool?>("REFERRALL_INDICATOR"));
            //toReturn.Add("PCPEligibleIndicator", x.Field<bool?>("PCP_ELIGIBLE_INDICATOR"));
            toReturn.Add("FloatProviderIndicator", x.Field<bool?>("FLOAT_PROVIDER_INDICATOR"));
            return toReturn;
        }
        #endregion

    }
}