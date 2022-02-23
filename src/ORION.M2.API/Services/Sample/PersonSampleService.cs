using Bogus;

namespace ORION.M2.API.Service.Sample;

public class PersonSampleService : ISampleService<DbContext.Models.Person>
{
    private readonly Random _random = new();

    public List<DbContext.Models.Person> Create(int amount, string locale)
    {
        return CreatePersonData(amount, locale);
    }

    private List<DbContext.Models.Person> CreatePersonData(int amount, string locale)
    {
        var locality = string.IsNullOrEmpty(locale) ? "nl" : locale;
        var company = GetRandomCompany();
        var samplePersons = new List<DbContext.Models.Person>();

        int id = 1;
        Faker<DbContext.Models.Person> faker = new Faker<DbContext.Models.Person>(locality)
            .CustomInstantiator(f => new DbContext.Models.Person())
            .RuleFor(p => p.ExternalId, (f, p) => $"SMP{id++}")
            .RuleFor(p => p.GivenName, f => f.Person.FirstName)
            .RuleFor(p => p.FamilyName, f => f.Person.LastName)
            .RuleFor(p => p.UserName, (f, p) => $"{p.GivenName.Substring(0, 1)}.{p.FamilyName}@{company}.com")
            .RuleFor(p => p.Initials, (f, p) => $"{p.GivenName.Substring(0, 1)}.{p.FamilyName.Substring(0, 1)}")
            .RuleFor(p => p.EmailAddress, (f, p) => $"{p.GivenName.Substring(0, 1)}.{p.FamilyName}@{company}.com")
            .RuleFor(p => p.Description, f => f.Lorem.Slug(3))
            .RuleFor(p => p.DateCreated, (f, p) => DateTime.Now.ToUniversalTime())
            .RuleFor(p => p.IsActive, f => f.PickRandomParam(true, false));

        return samplePersons = faker.Generate(amount);
    }

    private string GetRandomCompany()
    {
        var companies = Enum.GetValues(typeof(EnumCompanies));
        var companyName = companies.GetValue(_random.Next(companies.Length)).ToString();

        return companyName;
    }

    private enum EnumCompanies
    {
        Bluejam,
        Buzzdog,
        Topiczoom,
        Realcube,
        Yodoo,
        Fivechat,
        Katz,
        Edgeify,
        Skibox,
        Yabadooda,
        Kayveo,
        Skilith,
        Tavu,
        Browsebug,
        Kwimbee,
        Eabox,
        Quatz,
        Realpoint,
        Yata,
        Flashspan,
        Yadel,
        Eare,
        Divanoodle,
        Jabbersphere,
        Tagchat
    }
}