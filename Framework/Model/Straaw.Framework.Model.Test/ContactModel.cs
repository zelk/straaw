using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Straaw.Framework.Model;

namespace Straaw.Framework.Model.Test
{
	public class Contact : ImmutableModel<Contact, MutableContact>
	{
		public Contact(MutableContact contact)
		{
			this.MainPhoneNumber = contact.MainPhoneNumber;
			this.Names = ModelCopy<ContactNames, MutableContactNames>(contact.Names);
			this.Work = ModelCopy<ContactWork, MutableContactWork>(contact.Work);
			this.Groups = ModelCopy<ContactGroup, MutableContactGroup>(contact.Groups);
			this.PhoneNumbers = ModelCopy<ContactPhoneNumber, MutableContactPhoneNumber>(contact.PhoneNumbers);
			this.Services = ModelCopy<ContactService, MutableContactService>(contact.Services);
			this.EmailAddresses = ModelCopy<ContactEmailAddress, MutableContactEmailAddress>(contact.EmailAddresses);
			this.Urls = ModelCopy<ContactUrl, MutableContactUrl>(contact.Urls);
			this.Dates = ModelCopy<ContactDate, MutableContactDate>(contact.Dates);
			this.Addresses = ModelCopy<ContactAddress, MutableContactAddress>(contact.Addresses);
		}
		public override MutableContact ToMutable() { return new MutableContact(this); }
		
		public string									MainPhoneNumber { get; private set; }
		public ContactNames								Names { get; private set; }
		public ContactWork								Work { get; private set; }
		public ReadOnlyCollection<ContactGroup>			Groups { get; private set; }
		public ReadOnlyCollection<ContactPhoneNumber>	PhoneNumbers { get; private set; }
		public ReadOnlyCollection<ContactService>		Services { get; private set; }
		public ReadOnlyCollection<ContactEmailAddress>	EmailAddresses { get; private set; }
		public ReadOnlyCollection<ContactUrl>			Urls { get; private set; }
		public ReadOnlyCollection<ContactDate>			Dates { get; private set; }
		public ReadOnlyCollection<ContactAddress>		Addresses { get; private set; }
	}

	[DataContract(Name="contact")]
	public class MutableContact : MutableModel<Contact, MutableContact>
	{
		public MutableContact() : base() { }
		public MutableContact(Contact contact)
		{
			this.MainPhoneNumber = contact.MainPhoneNumber;
			this.Names = ModelCopy<ContactNames, MutableContactNames>(contact.Names);
			this.Work = ModelCopy<ContactWork, MutableContactWork>(contact.Work);
			this.Groups = ModelCopy<ContactGroup, MutableContactGroup>(contact.Groups);
			this.PhoneNumbers = ModelCopy<ContactPhoneNumber, MutableContactPhoneNumber>(contact.PhoneNumbers);
			this.Services = ModelCopy<ContactService, MutableContactService>(contact.Services);
			this.EmailAddresses = ModelCopy<ContactEmailAddress, MutableContactEmailAddress>(contact.EmailAddresses);
			this.Urls = ModelCopy<ContactUrl, MutableContactUrl>(contact.Urls);
			this.Dates = ModelCopy<ContactDate, MutableContactDate>(contact.Dates);
			this.Addresses = ModelCopy<ContactAddress, MutableContactAddress>(contact.Addresses);
		}
		public override Contact ToImmutable() { return new Contact(this); }

		[DataMember(Name="mainPhoneNumber")]
		public string MainPhoneNumber { get; set; }

		[DataMember(Name="names")]
		public MutableContactNames Names { get; private set; }
	
		[DataMember(Name="work")]
		public MutableContactWork Work { get; private set; }
		
		[DataMember(Name="groups")]
		public List<MutableContactGroup> Groups { get; private set; }
		
		[DataMember(Name="phoneNumbers")]
		public List<MutableContactPhoneNumber> PhoneNumbers { get; private set; }
		
		[DataMember(Name="services")]
		public List<MutableContactService> Services { get; private set; }
		
		[DataMember(Name="emailAddresses")]
		public List<MutableContactEmailAddress> EmailAddresses { get; private set; }
		
		[DataMember(Name="urls")]
		public List<MutableContactUrl> Urls { get; private set; }
		
		[DataMember(Name="dates")]
		public List<MutableContactDate> Dates { get; private set; }
		
		[DataMember(Name="addresses")]
		public List<MutableContactAddress> Addresses { get; private set; }
	}

	public interface Revealable
	{
		Reveal Reveal { get; }
	}

	public interface MutableRevealable
	{
		MutableReveal Reveal { get; set; }
	}

	public class Reveal : ImmutableModel<Reveal, MutableReveal>
	{
		public Reveal(MutableReveal reveal)
		{
			this.RevealType = reveal.RevealType;
			this.GroupHandles = base.ModelCopy(reveal.GroupHandles);
		}
		public override MutableReveal ToMutable() { return new MutableReveal(this); }

		public string RevealType { get; private set; }
		public ReadOnlyCollection<string> GroupHandles { get; private set; }
	}

	[DataContract]
	public class MutableReveal : MutableModel<Reveal, MutableReveal>
	{
		public MutableReveal() : base() { }
		public MutableReveal(Reveal reveal)
		{
			this.RevealType = reveal.RevealType;
			this.GroupHandles = base.ModelCopy(reveal.GroupHandles);
		}
		public override Reveal ToImmutable() { return new Reveal(this); }

		[DataMember(Name="revealType")]
		public string RevealType { get; set; }

		[DataMember(Name="groupHandles")]
		public List<string> GroupHandles { get; private set; }
	}

	public class ContactGroup : ImmutableModel<ContactGroup, MutableContactGroup>
	{
		public ContactGroup(MutableContactGroup contactGroup)
		{
			this.GroupHandle = contactGroup.GroupHandle;
			this.GroupName = contactGroup.GroupName;
			this.ObservedPhoneNumbers = base.ModelCopy(contactGroup.ObservedPhoneNumbers);
		}
		public override MutableContactGroup ToMutable() { return new MutableContactGroup(this); }

		public string GroupHandle { get; private set; }
		public string GroupName { get; private set; }
		public ReadOnlyCollection<string> ObservedPhoneNumbers { get; private set; }
	}

	[DataContract]
	public class MutableContactGroup : MutableModel<ContactGroup, MutableContactGroup>
	{
		public MutableContactGroup() : base() { }
		public MutableContactGroup(ContactGroup contactGroup)
		{
			this.GroupHandle = contactGroup.GroupHandle;
			this.GroupName = contactGroup.GroupName;
			this.ObservedPhoneNumbers = base.ModelCopy(contactGroup.ObservedPhoneNumbers);
		}
		public override ContactGroup ToImmutable() { return new ContactGroup(this); }

		[DataMember(Name="groupHandle")]
		public string GroupHandle { get; set; }

		[DataMember(Name="groupName")]
		public string GroupName { get; set; }
	
		[DataMember(Name="observedPhoneNumbers")]
		public List<string> ObservedPhoneNumbers { get; set; }
	}

	public class ContactNames : ImmutableModel<ContactNames, MutableContactNames>, Revealable
	{
		public ContactNames(MutableContactNames contactNames)
		{
			this.FirstName = contactNames.FirstName;
			this.LastName = contactNames.LastName;
			this.MiddleName = contactNames.MiddleName;
			this.NamePrefix = contactNames.FirstName;
			this.NameSuffix = contactNames.FirstName;
			this.NickName = contactNames.FirstName;
			this.FirstNamePhonetic = contactNames.FirstNamePhonetic;
			this.LastNamePhonetic = contactNames.LastNamePhonetic;
			this.MiddleNamePhonetic = contactNames.MiddleNamePhonetic;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactNames.Reveal);
		}
		public override MutableContactNames ToMutable() { return new MutableContactNames(this); }

		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string MiddleName { get; private set; }
		public string NamePrefix { get; private set; }
		public string NameSuffix { get; private set; }
		public string NickName { get; private set; }
		public string FirstNamePhonetic { get; private set; }
		public string LastNamePhonetic { get; private set; }
		public string MiddleNamePhonetic { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactNames : MutableModel<ContactNames, MutableContactNames>, MutableRevealable
	{
		public MutableContactNames() : base() { }
		public MutableContactNames(ContactNames contactNames)
		{
			this.FirstName = contactNames.FirstName;
			this.LastName = contactNames.LastName;
			this.MiddleName = contactNames.MiddleName;
			this.NamePrefix = contactNames.FirstName;
			this.NameSuffix = contactNames.FirstName;
			this.NickName = contactNames.FirstName;
			this.FirstNamePhonetic = contactNames.FirstNamePhonetic;
			this.LastNamePhonetic = contactNames.LastNamePhonetic;
			this.MiddleNamePhonetic = contactNames.MiddleNamePhonetic;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactNames.Reveal);
		}
		public override ContactNames ToImmutable() { return new ContactNames(this); }

		[DataMember(Name="firstName")]
		public string FirstName { get; set; }

		[DataMember(Name="lastName")]
		public string LastName { get; set; }

		[DataMember(Name="middleName")]
		public string MiddleName { get; set; }
	
		[DataMember(Name="namePrefix")]
		public string NamePrefix { get; set; }
		
		[DataMember(Name="nameSuffix")]
		public string NameSuffix { get; set; }
		
		[DataMember(Name="nickName")]
		public string NickName { get; set; }
		
		[DataMember(Name="firstNamePhonetic")]
		public string FirstNamePhonetic { get; set; }
		
		[DataMember(Name="lastNamePhonetic")]
		public string LastNamePhonetic { get; set; }
		
		[DataMember(Name="middleNamePhonetic")]
		public string MiddleNamePhonetic { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactWork : ImmutableModel<ContactWork, MutableContactWork>, Revealable
	{
		public ContactWork(MutableContactWork contactWork)
		{
			this.OrganizationName = contactWork.OrganizationName;
			this.JobTitle = contactWork.JobTitle;
			this.DepartmentName = contactWork.DepartmentName;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactWork.Reveal);
		}
		public override MutableContactWork ToMutable() { return new MutableContactWork(this); }

		public string OrganizationName { get; private set; }
		public string JobTitle { get; private set; }
		public string DepartmentName { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactWork : MutableModel<ContactWork, MutableContactWork>, MutableRevealable
	{
		public MutableContactWork() : base() { }
		public MutableContactWork(ContactWork contactWork)
		{
			this.OrganizationName = contactWork.OrganizationName;
			this.JobTitle = contactWork.JobTitle;
			this.DepartmentName = contactWork.DepartmentName;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactWork.Reveal);
		}
		public override ContactWork ToImmutable() { return new ContactWork(this); }

		[DataMember(Name="organizationName")]
		public string OrganizationName { get; set; }

		[DataMember(Name="jobTitle")]
		public string JobTitle { get; set; }
	
		[DataMember(Name="departmentName")]
		public string DepartmentName { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactAddress : ImmutableModel<ContactAddress, MutableContactAddress>, Revealable
	{
		public ContactAddress(MutableContactAddress contactAddress)
		{
			this.Label = contactAddress.Label;
			this.StreetAddresses = base.ModelCopy(contactAddress.StreetAddresses);
			this.StateName = contactAddress.StateName;
			this.ZipCode = contactAddress.ZipCode;
			this.CityName = contactAddress.CityName;
			this.CountryName = contactAddress.CountryName;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactAddress.Reveal);
		}
		public override MutableContactAddress ToMutable() { return new MutableContactAddress(this); }

		public string Label { get; private set; }
		public ReadOnlyCollection<string> StreetAddresses { get; private set; }
		public string StateName { get; private set; }
		public string ZipCode { get; private set; }
		public string CityName { get; private set; }
		public string CountryName { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactAddress : MutableModel<ContactAddress, MutableContactAddress>, MutableRevealable
	{
		public MutableContactAddress() : base() { }
		public MutableContactAddress(ContactAddress contactAddress)
		{
			this.Label = contactAddress.Label;
			this.StreetAddresses = base.ModelCopy(contactAddress.StreetAddresses);
			this.StateName = contactAddress.StateName;
			this.ZipCode = contactAddress.ZipCode;
			this.CityName = contactAddress.CityName;
			this.CountryName = contactAddress.CountryName;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactAddress.Reveal);
		}
		public override ContactAddress ToImmutable() { return new ContactAddress(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="streetAddresses")]
		public List<string> StreetAddresses { get; set; }
	
		[DataMember(Name="stateName")]
		public string StateName { get; set; }
		
		[DataMember(Name="zipCode")]
		public string ZipCode { get; set; }
		
		[DataMember(Name="cityName")]
		public string CityName { get; set; }
		
		[DataMember(Name="countryName")]
		public string CountryName { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactService : ImmutableModel<ContactService, MutableContactService>, Revealable
	{
		public ContactService(MutableContactService service)
		{
			this.Label = service.Label;
			this.ServiceName = service.ServiceName;
			this.UserName = service.UserName;
			this.ServiceUrl = service.ServiceUrl;
			this.UserIdentifier = service.UserIdentifier;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(service.Reveal);
		}
		public override MutableContactService ToMutable() { return new MutableContactService(this); }

		public string Label { get; private set; }
		public string ServiceName { get; private set; }
		public string UserName { get; private set; }
		public string ServiceUrl { get; private set; }
		public string UserIdentifier { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactService : MutableModel<ContactService, MutableContactService>, MutableRevealable
	{
		public MutableContactService() : base() { }
		public MutableContactService(ContactService service)
		{
			this.Label = service.Label;
			this.ServiceName = service.ServiceName;
			this.UserName = service.UserName;
			this.ServiceUrl = service.ServiceUrl;
			this.UserIdentifier = service.UserIdentifier;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(service.Reveal);
		}
		public override ContactService ToImmutable() { return new ContactService(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="serviceName")]
		public string ServiceName { get; private set; }

		[DataMember(Name="userName")]
		public string UserName { get; private set; }
	
		[DataMember(Name="serviceUrl")]
		public string ServiceUrl { get; private set; }
		
		[DataMember(Name="userIdentifier")]
		public string UserIdentifier { get; private set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactEmailAddress : ImmutableModel<ContactEmailAddress, MutableContactEmailAddress>, Revealable
	{
		public ContactEmailAddress(MutableContactEmailAddress emailAddress)
		{
			this.Label = emailAddress.Label;
			this.EmailAddress = emailAddress.EmailAddress;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(emailAddress.Reveal);
		}
		public override MutableContactEmailAddress ToMutable() { return new MutableContactEmailAddress(this); }

		public string Label { get; private set; }
		public string EmailAddress { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactEmailAddress : MutableModel<ContactEmailAddress, MutableContactEmailAddress>, MutableRevealable
	{
		public MutableContactEmailAddress() : base() { }
		public MutableContactEmailAddress(ContactEmailAddress emailAddress)
		{
			this.Label = emailAddress.Label;
			this.EmailAddress = emailAddress.EmailAddress;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(emailAddress.Reveal);
		}
		public override ContactEmailAddress ToImmutable() { return new ContactEmailAddress(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="emailAddress")]
		public string EmailAddress { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactUrl : ImmutableModel<ContactUrl, MutableContactUrl>, Revealable
	{
		public ContactUrl(MutableContactUrl contactUrl)
		{
			this.Label = contactUrl.Label;
			this.Url = contactUrl.Url;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactUrl.Reveal);
		}
		public override MutableContactUrl ToMutable() { return new MutableContactUrl(this); }

		public string Label { get; private set; }
		public string Url { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactUrl : MutableModel<ContactUrl, MutableContactUrl>, MutableRevealable
	{
		public MutableContactUrl() : base() { }
		public MutableContactUrl(ContactUrl contactUrl)
		{
			this.Label = contactUrl.Label;
			this.Url = contactUrl.Url;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactUrl.Reveal);
		}
		public override ContactUrl ToImmutable() { return new ContactUrl(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="url")]
		public string Url { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactDate : ImmutableModel<ContactDate, MutableContactDate>, Revealable
	{
		public ContactDate(MutableContactDate contactDate)
		{
			this.Label = contactDate.Label;
			this.Date = contactDate.Date;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactDate.Reveal);
		}
		public override MutableContactDate ToMutable() { return new MutableContactDate(this); }

		public string Label { get; private set; }
		public DateTime Date { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactDate : MutableModel<ContactDate, MutableContactDate>, MutableRevealable
	{
		public MutableContactDate() : base() { }
		public MutableContactDate(ContactDate contactDate)
		{
			this.Label = contactDate.Label;
			this.Date = contactDate.Date;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactDate.Reveal);
		}
		public override ContactDate ToImmutable() { return new ContactDate(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="date")]
		public DateTime Date { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}

	public class ContactPhoneNumber : ImmutableModel<ContactPhoneNumber, MutableContactPhoneNumber>, Revealable
	{
		public ContactPhoneNumber(MutableContactPhoneNumber contactPhoneNumber)
		{
			this.Label = contactPhoneNumber.Label;
			this.PhoneNumber = contactPhoneNumber.PhoneNumber;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactPhoneNumber.Reveal);
		}
		public override MutableContactPhoneNumber ToMutable() { return new MutableContactPhoneNumber(this); }

		public string Label { get; private set; }
		public string PhoneNumber { get; private set; }
		public Reveal Reveal { get; private set; }
	}

	[DataContract]
	public class MutableContactPhoneNumber : MutableModel<ContactPhoneNumber, MutableContactPhoneNumber>, MutableRevealable
	{
		public MutableContactPhoneNumber() : base() { }
		public MutableContactPhoneNumber(ContactPhoneNumber contactPhoneNumber)
		{
			this.Label = contactPhoneNumber.Label;
			this.PhoneNumber = contactPhoneNumber.PhoneNumber;
			this.Reveal = base.ModelCopy<Reveal, MutableReveal>(contactPhoneNumber.Reveal);
		}
		public override ContactPhoneNumber ToImmutable() { return new ContactPhoneNumber(this); }

		[DataMember(Name="label")]
		public string Label { get; set; }

		[DataMember(Name="phoneNumber")]
		public string PhoneNumber { get; set; }

		[DataMember(Name="reveal")]
		public MutableReveal Reveal { get; set; }
	}
}
