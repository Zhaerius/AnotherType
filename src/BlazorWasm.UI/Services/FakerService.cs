using Bogus;

namespace POC_AnotherType.Services;

public class FakerService
{
    public string GetText()
    {
        var faker = new Faker(locale: "fr");
        List<string> phrases = [];
        
        for (int i = 0; i < 2; i++)
        {
            phrases.Add(faker.Hacker.Phrase());
        }
        
        return string.Join(" ", phrases);
    }
}