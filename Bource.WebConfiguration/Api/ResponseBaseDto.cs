using AutoMapper;
using Bource.Models;
using Bource.WebConfiguration.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace Bource.WebConfiguration.Api
{
    public abstract class ResponseBaseDto<TDto, TEntity, TKey> : ICustomMapping
        where TDto : class, new()
        where TEntity : IEntity, new()
    {
        [Display(Name = "ردیف")]
        public TKey Id { get; set; }

        public TEntity ToEntity(IMapper mapper)
        {
            return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        }

        public TEntity ToEntity(IMapper mapper, TEntity entity)
        {
            return mapper.Map(CastToDerivedClass(mapper, this), entity);
        }

        public static TDto FromEntity(IMapper mapper, TEntity model)
        {
            return mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(IMapper mapper, ResponseBaseDto<TDto, TEntity, TKey> baseInstance)
        {
            return mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            //Ignore any property of source (like Post.Author) that dose not contains in destination
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
            CustomReverseMappings(mappingExpression);
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }

        public virtual void CustomReverseMappings(IMappingExpression<TDto, TEntity> mapping)
        {
        }
    }

    public abstract class ResponseBaseDto<TDto, TEntity> : ResponseBaseDto<TDto, TEntity, string>
        where TDto : class, new()
        where TEntity : BaseEntity<string>, new()
    {
    }
}