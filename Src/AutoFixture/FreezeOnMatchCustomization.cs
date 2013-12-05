using System;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture
{
    public class FreezeOnMatchCustomization : ICustomization
    {
        private readonly Type targetType;
        private readonly string identifier;
        private readonly Matching matchBy;
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
        }

        private void MatchByExactType()
        {
            if (this.matchBy.HasFlag(Matching.ExactType))
            {
                FreezeTypeForMatchingRequests(new ExactTypeSpecification(this.targetType));
            }
        }

        private void MatchByBaseType()
        {
            if (this.matchBy.HasFlag(Matching.BaseType))
            {
                FreezeTypeForMatchingRequests(new BaseTypeSpecification(this.targetType));
            }
        }

        private void MatchByImplementedInterfaces()
        {
            if (this.matchBy.HasFlag(Matching.ImplementedInterfaces))
            {
                FreezeTypeForMatchingRequests(new BaseTypeSpecification(this.targetType));
            }
        }

        private void MatchByPropertyName()
        {
            if (this.matchBy.HasFlag(Matching.PropertyName))
            {
                FreezeTypeForMatchingRequests(new PropertyNameSpecification(this.identifier));
            }
        }

        private void FreezeTypeForMatchingRequests(IRequestSpecification criteria)
        {
            this.fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    FreezeTargetType(),
                    criteria));
        }

        private ISpecimenBuilder FreezeTargetType()
        {
            var specimen = new SpecimenContext(this.fixture).Resolve(this.targetType);
            return new FixedBuilder(specimen);
        }
    }
}
