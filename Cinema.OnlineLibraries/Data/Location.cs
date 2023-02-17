namespace Cinema.OnlineLibraries.Data
{
  public class LocationOld
  {
    private readonly string _postalCode;
    private readonly string _country;

    public enum Country
    {
      AR,
      AU,
      CA,
      CL,
      DE,
      ES,
      FR,
      IT,
      MX,
      NZ,
      PT,
      US,
      UK
    }

    public LocationOld(Country country, string postalCode)
    {
      _country = country.ToString();
      _postalCode = postalCode;
    }

    public override string ToString()
    {
      return _country + "/" + _postalCode;
    }
  }
}
