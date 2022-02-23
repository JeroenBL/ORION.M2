namespace ORION.M2.API.Service.Sample;

public interface ISampleService<T>
{
    // Create
    List<T> Create(int amount, string locale);
}
