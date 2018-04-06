using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ProviderHubService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IProviderHubService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        Provider GetProviderByID(int providerID);

        [OperationContract]
        List<Language> GetProviderLanguageByID(int providerID);

        [OperationContract]
        List<Credential> GetProviderCredentialByID(int providerID);
        
        [OperationContract]
        Facility GetFacilityByID(int facilityID);

        [OperationContract]
        Address GetAddressByFacilityID(int facilityID);

        [OperationContract]
        List<Provider> GetProviderList(string searchValue);

        [OperationContract]
        List<Facility> GetFacilityList(string searchValue);

        [OperationContract]
        SearchResults SearchForValue(string searchValue);

        [OperationContract]
        List<FacilityProviderRelationship> GetFacilityProviderRelationshipList(string searchValue);

        [OperationContract]
        FacilityProviderRelationship GetFacilityProviderRelationshipByID(int relationshipID);

        [OperationContract]
        int SaveProviderDetail(Provider provider);

        [OperationContract]
        int SaveFacility(Facility facility);

        [OperationContract]
        int SaveFacilityProviderRelationship(FacilityProviderRelationship relationship);

        [OperationContract]
        int SaveAddress(Address address);

        [OperationContract]
        int MapAddressToFacility(int facilityID, int addressID, string createdBy);

        [OperationContract]
        List<Language> GetLanguageList();

        [OperationContract]
        List<Credential> GetCredentialList();

        [OperationContract]
        List<Vendor> GetVendorList(string searchValue);

        [OperationContract]
        int SaveVendor(Vendor vendor);

        [OperationContract]
        int MapAddressToVendor(int vendorID, int addressID, string createdBy);

        [OperationContract]
        Vendor GetVendorByID(int vendorID);

        [OperationContract]
        List<Address> GetAddressByVendorID(int vendorID);

        [OperationContract]
        int MapFacilityToVendor(int facilityID, int vendorID, string createdBy);
        
        [OperationContract]
        bool SaveLanguageByProviderID(int providerID, List<Language> languages);

        [OperationContract]
        bool SaveCredentialByProviderID(int providerID, List<Credential> credentials);
        
        [OperationContract]
        List<FacilityProviderRelationship> AdvancedSearch(Dictionary<string, List<string>> args);

        [OperationContract]
        List<BehavioralHealthAttribute> GetBehavioralHealthAttributeByID(BHAttributeType bHAttributeType);

        [OperationContract]
        List<BehavioralHealthAttribute> GetBHAttributeByRelationshipID(int relationshipID);

        [OperationContract]
        bool SaveBHAttributeToRelationship(int relationshipID, List<BehavioralHealthAttribute> bhAttributeList);


    }
}

