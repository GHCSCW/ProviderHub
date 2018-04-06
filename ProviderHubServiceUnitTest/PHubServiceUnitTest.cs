using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProviderHubServiceUnitTest.ProviderHubService;
using System.Collections.Generic;
using System.Linq;


namespace ProviderHubServiceUnitTest
{
    [TestClass]
    public class PHubServiceUnitTest
    {
        [TestMethod]
        public void ValidateProviderByID()
        {

            Provider provider;

            using (ProviderHubService.ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                provider = pHub.GetProviderByID(1);
            }

            Assert.AreEqual(provider.ExternalProviderName, "Nina Bartel");
        }

        [TestMethod]
        public void ValidateFacilityByID()
        {
            Facility facility;

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                facility = pHub.GetFacilityByID(4);
            }

            Assert.AreEqual(facility.FacilityName, "Catholic Charities");
        }

        [TestMethod]
        public void SearchForValue()
        {
            SearchResults sr;
            string searchValue = "Barbara White";

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                sr = pHub.SearchForValue(searchValue);
            }

            foreach (FacilityProviderRelationship fpRelationship in sr.FacilityProviderRelationships)
            {
                Assert.AreEqual(fpRelationship.Provider.Gender, ProviderGender.Female);
            }
        }

        [TestMethod]
        public void SearchForFacility()
        {
            List<Facility> facilities = new List<Facility>();

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                facilities = pHub.GetFacilityList("").ToList();
            }

            Assert.IsTrue(facilities.Count > 0);
        }
        
        [TestMethod]
        public void GetFacilityProviderRelationshipByID()
        {
            FacilityProviderRelationship relationship;
            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                relationship = pHub.GetFacilityProviderRelationshipByID(100);
            }
            Assert.IsNotNull(value: relationship);
        }

        [TestMethod]
        public void SaveNewFacility()
        {
            int facilityID =0;

            Facility newFacility = new Facility
            {
                FacilityName = "Testing Facility Family Counseling",
                NPI = null,
                ExternalID = null,
                InternalNotes = "Testing Facility",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                facilityID = pHub.SaveFacility(newFacility);
            }

            Assert.IsTrue(facilityID != 0);
        }

        [TestMethod]
        public void SaveUpdatedFacility()
        {
            int facilityID = 47;

            Facility facility = new Facility
            {
                ID = facilityID,
                FacilityName = "Testing Number 2",
                NPI = null,
                ExternalID = null,
                InternalNotes = "Sauk County",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                facilityID = pHub.SaveFacility(facility);
            }

            Assert.IsTrue(facilityID == facility.ID);
        }

        [TestMethod]
        public void SaveNewProviderDetail()
        {
            int providerID = 0;

            Provider newProvider = new Provider
            {
                EpicProviderID = null,
                NPI = null,
                FirstName = "Jose",
                MiddleName = "",
                LastName = "Mendez",
                ExternalProviderID = null,
                ExternalProviderName = null,
                DateOfBirth = null,
                Gender = ProviderGender.Male,
                CSP_Indicator = false,
                MedicareIndicator = false,
                MedicarePTAN = null,
                MedicareEffectiveDate = null,
                MedicareTerminationDate = null,
                MedicaidIndicator = false,
                MedicaidProviderID = null,
                EffectiveDate = null,
                TerminationDate = null,
                InternalNotes = "Testing Provider Jose Mendez",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                providerID = pHub.SaveProviderDetail(newProvider);
            }

            Assert.IsTrue(providerID != 0);
        }

        [TestMethod]
        public void SaveUpdateProviderDetail()
        {
            int updatedProviderID = 339; ;

            Provider updatedProvider = new Provider
            {
                ID = updatedProviderID,
                EpicProviderID = null,
                NPI = "654987321",
                FirstName = "Monica",
                MiddleName = "",
                LastName = "Monje",
                ExternalProviderID = null,
                ExternalProviderName = "Monje, Monica",
                DateOfBirth = null,
                Gender = ProviderGender.Female,
                CSP_Indicator = false,
                MedicareIndicator = false,
                MedicarePTAN = null,
                MedicareEffectiveDate = null,
                MedicareTerminationDate = null,
                MedicaidIndicator = false,
                MedicaidProviderID = null,
                EffectiveDate = null,
                TerminationDate = null,
                InternalNotes = "Testing Provider Monica II",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                updatedProviderID = pHub.SaveProviderDetail(updatedProvider);
            }

            Assert.IsTrue(updatedProviderID == updatedProvider.ID);
        }

        [TestMethod]
        public void SaveNewAddress()
        {
            int addressID = 0;

            Address newAddress = new Address
            {
                AddressLine1 = "15099 Chipper",
                AddressLine2 = null,
                AddressType = AddressType.ClinicalPracticeServiceLocation,
                AlternatePhoneNumber = "14147318066",
                City = "Sun Prairie",
                ContactFirstName = "Alex",
                ContactLastName = "Peasley",
                County = null,
                Email = "peaslead@gmail.com",
                FaxNumber = null,
                PhoneNumber = "WI",
                Region = null,
                State = "WI",
                Website = null,
                ZipCode = "53590",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                addressID = pHub.SaveAddress(newAddress);
            }

            Assert.IsTrue(addressID != 0);
        }

        [TestMethod]
        public void SaveUpdateAddress()
        {
            int addressID = 0;

            Address updateAddress = new Address
            {
                ID = 45,
                AddressLine1 = "2910 New Pinery Rd",
                AddressLine2 = "Unit A2",
                AddressType = AddressType.ClinicalPracticeServiceLocation,
                AlternatePhoneNumber = null,
                City = "Portage",
                ContactFirstName = null,
                ContactLastName = null,
                County = null,
                Email = null,
                FaxNumber = "608-745-4990",
                PhoneNumber = "608-745-4900",
                Region = "Sauk County",
                State = "WI",
                Website = null,
                ZipCode = "53907",
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                addressID = pHub.SaveAddress(updateAddress);
            }

            Assert.IsTrue(addressID != 0);
        }

        [TestMethod]
        public void MapAddressToFacility()
        {
            int retVal = 0;
            int facilityID = 47;
            int addressID = 47;

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.MapAddressToFacility(facilityID, addressID, Environment.UserName);
            }

            Assert.IsTrue(retVal != 0);
        }

        public void SaveUpdateFacilityProviderRelationship()
        {
            int relationshipID = 0;

            //Facility data
            Facility facility = new Facility
            {
                ID = 46,
                FacilityName = "Goodman's Behavioral Health Clinic",
                NPI = null,
                ExternalID = null,
                InternalNotes = "Sauk County",
                LastUpdatedDate = DateTime.Today,
                LastUpdatedBy = Environment.UserName
            };

            //Provider data
            Provider provider = new Provider
            {
                ID = 339,
                EpicProviderID = null,
                NPI = "654987321",
                FirstName = "Monica",
                MiddleName = "",
                LastName = "Monje",
                ExternalProviderID = null,
                ExternalProviderName = "Monje, Monica",
                DateOfBirth = null,
                Gender = ProviderGender.Female,
                CSP_Indicator = false,
                MedicareIndicator = false,
                MedicarePTAN = null,
                MedicareEffectiveDate = null,
                MedicareTerminationDate = null,
                MedicaidIndicator = false,
                MedicaidProviderID = null,
                EffectiveDate = null,
                TerminationDate = null,
                InternalNotes = "Testing Provider Monica II",
                LastUpdatedBy = Environment.UserName
            };

            //Facility Relationship data
            FacilityProviderRelationship updateFacilityProviderRelationship = new FacilityProviderRelationship
            {
                RelationshipID = 0,
                Facility = facility,
                Provider = provider,
                ExternalProviderIndicator = false,
                AcceptingNewPatientIndicator = true,
                PrescriberIndicator = true,
                ReferralIndicator = true,
                FloatProviderIndicator = false,
                EffectiveDate = Convert.ToDateTime("01/01/2017"),
                TerminationDate = Convert.ToDateTime("12/31/2999"),
                ProviderEmail = null,
                ProviderPhoneNumber = null,
                ProviderExtensionNumber = null,
                InternalNotes = null,
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                relationshipID = pHub.SaveFacilityProviderRelationship(updateFacilityProviderRelationship);
            }

            Assert.IsTrue(relationshipID != 0);
        }


        [TestMethod]
        public void GetLanguageList()
        {
            List<Language> languages = new List<Language>();

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                languages = pHub.GetLanguageList().ToList();
            }

            Assert.IsTrue(languages.Count == 3);
        }

        [TestMethod]
        public void GetCredentialList()
        {
            List<Credential> credentialList = new List<Credential>();

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                credentialList = pHub.GetCredentialList().ToList();
            }

            Assert.IsTrue(credentialList.Count == 293);

        }

        [TestMethod]
        public void GetVendorList()
        {
            List<Vendor> vendors = new List<Vendor>();
            
            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                vendors = pHub.GetVendorList("").ToList();
            }

            Assert.IsTrue(vendors.Count == 1);
        }

        [TestMethod]
        public void SaveNewVendor()
        {
            int vendorID = 0;

            Vendor newVendor = new Vendor
            {
                ID = 0,
                VendorName = "Cambridge Counseling Center.",
                EPICVendorID = null,
                ExternalID = null,
                NPI = null,
                TaxID = null,
                InternalNotes = null,
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                vendorID = pHub.SaveVendor(newVendor);
            }

            Assert.IsTrue(vendorID != 0);
        }

        [TestMethod]
        public void SaveUpdatedVendor()
        {
            int vendorID = 0;

            Vendor updatedVendor = new Vendor
            {
                ID = 3,
                VendorName = "Cambridge Counseling Center",
                EPICVendorID = null,
                ExternalID = null,
                NPI = null,
                TaxID = null,
                InternalNotes = null,
                LastUpdatedBy = Environment.UserName
            };

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                vendorID = pHub.SaveVendor(updatedVendor);
            }

            Assert.IsTrue(vendorID != 0);
        }

        [TestMethod]
        public void MapAddressToVendor()
        {
            int retVal = 0;
            int vendorID = 2;
            int addressID = 54;

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.MapAddressToVendor(vendorID, addressID, Environment.UserName);
            }

            Assert.IsTrue(retVal != 0);
        }

        [TestMethod]
        public void MapFacilityToVendor()
        {
            int retVal = 0;
            int facilityID = 2;
            int vendorID = 2;

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.MapFacilityToVendor(facilityID, vendorID, Environment.UserName);
            }

            Assert.IsTrue(retVal != 0);
        }

        [TestMethod]
        public void SaveLangaugeByProviderID()
        {
            bool retVal;
            int providerID = 2;

            List<Language> languages = new List<Language>();

            for (int i = 1; i <= 2; i++)
            {
                Language language = new Language();

                if (i == 1)
                {
                    language.ID = i;
                    language.Name = "English";
                    language.SequenceNumber = i;
                    language.CreatedDate = DateTime.Today;
                    language.Status = true;
                    
                }
                else
                {
                    language.ID = i;
                    language.Name = "Spanish";
                    language.SequenceNumber = i;
                    language.CreatedDate = DateTime.Today;
                    language.Status = true;
                }

                languages.Add(language);
            }

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.SaveLanguageByProviderID(providerID, languages.ToArray());
            }

            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void SaveCredentialByProviderID()
        {
            bool retVal;
            int providerID = 1;

            List<Credential> credentials = new List<Credential>();

            for (int i = 1; i <= 2; i++)
            {
                Credential credential = new Credential();

                if (i == 1)
                {
                    credential.ID = 242;
                    credential.Value = "PHD";
                    credential.SequenceNumber = i;
                    credential.CreatedDate = DateTime.Today;
                    credential.Status = true;
                }
                else
                {
                    credential.ID = 18;
                    credential.Value = "ABPMR";
                    credential.SequenceNumber = i;
                    credential.CreatedDate = DateTime.Today;
                    credential.Status = true;
                }

                credentials.Add(credential);
            }

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.SaveCredentialByProviderID(providerID, credentials.ToArray());
            }

            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void AdvancedSearch()
        {
            List<FacilityProviderRelationship> relationshipList = new List<FacilityProviderRelationship>();
           
            string[] gender = new string[] { "1" };
            string[] language = new string[] { "1", "2" };
            
            Dictionary<string, string[]> argList = new Dictionary<string, string[]>()
            {
                {"Gender", gender},
                {"Language", language}
            };
            
            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                relationshipList = pHub.AdvancedSearch(argList).ToList();
            }

            Assert.IsTrue(relationshipList.Count > 0);
        }

        [TestMethod]
        public void GetBehavioralHealthAttributeByID()
        {
            List<BehavioralHealthAttribute> attributes = new List<BehavioralHealthAttribute>();

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                attributes = pHub.GetBehavioralHealthAttributeByID(BHAttributeType.Ages).ToList();
            }

            Assert.IsTrue(attributes.Count > 0);
        }

        [TestMethod]
        public void GetBHAttributeByRelationshipID()
        {
            List<BehavioralHealthAttribute> attributes = new List<BehavioralHealthAttribute>();

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                attributes = pHub.GetBHAttributeByRelationshipID(122).ToList();
            }

            Assert.IsTrue(attributes.Count > 0);
        }

        [TestMethod]
        public void SaveBHealthAttributeToRelationship()
        {
            bool retVal;
            int relationshipID = 1;

            List<BehavioralHealthAttribute> bhAttributeList = new List<BehavioralHealthAttribute>();

            for (int i = 1; i <= 2; i++)
            {
                BehavioralHealthAttribute attribute = new BehavioralHealthAttribute();

                if (i == 1)
                {
                    //Delete specific relationship/bh attribute mapping
                    attribute.MappingID = 13;
                    attribute.SetID = 56;
                    attribute.BHSpecialtyType = BHAttributeType.Other;
                    attribute.Status = false;
                }
                else
                {
                    //insert new value
                    attribute.MappingID = 0;
                    attribute.SetID = 55;
                    attribute.BHSpecialtyType = BHAttributeType.Other;
                    attribute.Status = true;
                }

                bhAttributeList.Add(attribute);
            }

            using (ProviderHubServiceClient pHub = new ProviderHubServiceClient())
            {
                retVal = pHub.SaveBHAttributeToRelationship(relationshipID, bhAttributeList.ToArray());
            }

            Assert.IsTrue(retVal);
        }

    }
}