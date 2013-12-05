using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture.Xunit
{
    public class FreezeOnMatchCustomization : ICustomization
    {
        private readonly Type targetType;
        private readonly string identifier;
        private readonly Matching matchBy;
        private readonly ICollection<IRequestSpecification> filters;
        private IFixture fixture;

        public FreezeOnMatchCustomization(
            Type targetType,
            string identifier = null,
            Matching matchBy = Matching.ExactType)
        {
            Require.IsNotNull(targetType, "targetType");

            this.targetType = targetType;
            this.identifier = identifier;
            this.matchBy = matchBy;
            this.filters = new Collection<IRequestSpecification>();
        }

        public Type TargetType
        {
            get { return this.targetType; }
        }

        public Matching MatchBy
        {
            get { return this.matchBy; }
        }

        public void Customize(IFixture fixture)
        {
            Require.IsNotNull(fixture, "fixture");

            this.fixture = fixture;
            MatchSpecimenByType();
            MatchSpecimenByName();
            FreezeTypeForMatchingRequests();
        }

        private void MatchSpecimenByType()
        {
            MatchByExactType();
            MatchByBaseType();
            MatchByImplementedInterfaces();
        }

        private void MatchSpecimenByName()
        {
            MatchByPropertyName();
            MatchByParameterName();
        }

        private void MatchByExactType()
        {
            if (ShouldMatchBy(Matching.ExactType))
            {
                filters.Add(new ExactTypeSpecification(this.targetType));
            }
        }

        private void MatchByBaseType()
        {
            if (ShouldMatchBy(Matching.BaseType))
            {
                filters.Add(new BaseTypeSpecification(this.targetType));
            }
        }

        private void MatchByImplementedInterfaces()
        {
            if (ShouldMatchBy(Matching.ImplementedInterfaces))
            {
                filters.Add(new BaseTypeSpecification(this.targetType));
            }
        }

        private void MatchByPropertyName()
        {
            if (ShouldMatchBy(Matching.PropertyName))
            {
                filters.Add(new PropertyNameSpecification(this.identifier));
            }
        }

        private void MatchByParameterName()
        {
            if (ShouldMatchBy(Matching.ParameterName))
            {
                filters.Add(new ParameterNameSpecification(this.identifier));
            }
        }

        private bool ShouldMatchBy(Matching criteria)
        {
            return criteria.HasFlag(this.matchBy);
        }

        private void FreezeTypeForMatchingRequests()
        {
            this.fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    FreezeTargetType(),
                    new OrRequestSpecification(filters)));
        }

        private ISpecimenBuilder FreezeTargetType()
        {
            var specimen = new SpecimenContext(this.fixture).Resolve(this.targetType);
            return new FixedBuilder(specimen);
        }
    }
}
