using AutoMapper;

namespace Bource.WebConfiguration.CustomMapping
{
    public interface ICustomMapping
    {
        void CreateMappings(Profile profile);
    }
}