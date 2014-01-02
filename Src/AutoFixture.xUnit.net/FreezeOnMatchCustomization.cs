using System;
using Ploeh.AutoFixture.Dsl;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture.Xunit
{
    public class FreezeOnMatchCustomization<T> : ICustomization
    {
        private readonly Type targetType;
        private readonly string identifier;
        private readonly Matching matchBy;
        private IFixture fixture;
        private IMatchComposer<T> filter;

        public FreezeOnMatchCustomization(
            Matching matchBy = Matching.ExactType,
            string identifier = null)
        {
            this.targetType = typeof(T);
            this.matchBy = matchBy;
            this.identifier = identifier;
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
            this.filter = new MatchComposer<T>(FreezeTargetType());

            MatchByType();
            MatchByIdentifier();
            FreezeTypeForMatchingRequests();
        }

        private ISpecimenBuilder FreezeTargetType()
        {
            var specimen = new SpecimenContext(this.fixture).Resolve(this.targetType);
            return new FixedBuilder(specimen);
        }

        private void MatchByType()
        {
            MatchByExactType();
            MatchByBaseType();
            MatchByImplementedInterfaces();
        }

        private void MatchByIdentifier()
        {
            MatchByPropertyName();
            MatchByParameterName();
            MatchByFieldName();
        }

        private void MatchByExactType()
        {
            if (ShouldMatchBy(Matching.ExactType))
            {
                filter = filter.ByExactType();
            }
        }

        private void MatchByBaseType()
        {
            if (ShouldMatchBy(Matching.BaseType))
            {
                filter = filter.ByBaseType();
            }
        }

        private void MatchByImplementedInterfaces()
        {
            if (ShouldMatchBy(Matching.ImplementedInterfaces))
            {
                filter = filter.ByBaseType();
            }
        }

        private void MatchByPropertyName()
        {
            if (ShouldMatchBy(Matching.PropertyName))
            {
                filter = filter.ByPropertyName(this.identifier);
            }
        }

        private void MatchByParameterName()
        {
            if (ShouldMatchBy(Matching.ParameterName))
            {
                filter = filter.ByArgumentName(this.identifier);
            }
        }

        private void MatchByFieldName()
        {
            if (ShouldMatchBy(Matching.FieldName))
            {
                filter = filter.ByFieldName(this.identifier);
            }
        }

        private bool ShouldMatchBy(Matching criteria)
        {
            return criteria.HasFlag(this.matchBy);
        }

        private void FreezeTypeForMatchingRequests()
        {
            this.fixture.Customize<T>(c => filter);
        }
    }
}
