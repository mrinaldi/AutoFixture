using System;
using System.Collections;
using Ploeh.AutoFixture.Kernel;
using Ploeh.TestTypeFoundation;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.AutoFixture.Xunit.UnitTest
{
    public class FreezeOnMatchCustomizationTest
    {
        [Fact]
        public void SutShouldBeCustomization()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeOnMatchCustomization<object>();
            // Verify outcome
            Assert.IsAssignableFrom<ICustomization>(sut);
            // Teardown
        }

        [Fact]
        public void InitializeWithTargetTypeShouldSetCorrespondingProperty()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeOnMatchCustomization<object>();
            // Verify outcome
            Assert.Equal(typeof(object), sut.TargetType);
        }

        [Fact]
        public void InitializeWithTargetTypeShouldSetMatchByPropertyToExactType()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeOnMatchCustomization<string>();
            // Verify outcome
            Assert.Equal(Matching.ExactType, sut.MatchBy);
        }

        [Fact]
        public void InitializeWithTargetTypeAndMatchByShouldSetCorrespondingProperties()
        {
            // Fixture setup
            var matcher = Matching.BaseType;
            // Exercise system
            var sut = new FreezeOnMatchCustomization<object>(matcher);
            // Verify outcome
            Assert.Equal(typeof(object), sut.TargetType);
            Assert.Equal(matcher, sut.MatchBy);
        }

        [Fact]
        public void CustomizeWithNullFixtureShouldThrowArgumentNullException()
        {
            // Fixture setup
            var sut = new FreezeOnMatchCustomization<object>();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                sut.Customize(null));
            // Teardown
        }

        [Theory]
        [InlineData(typeof(object), typeof(object), true)]
        [InlineData(typeof(string), typeof(string), true)]
        [InlineData(typeof(int), typeof(int), true)]
        [InlineData(typeof(object), typeof(bool), false)]
        [InlineData(typeof(int), typeof(string), false)]
        public void FreezeByMatchingExactTypeShouldReturnTheRightSpecimen(
            Type frozenType,
            Type requestedType,
            bool areSameSpecimen)
        {
            // Fixture setup
            var fixture = new Fixture();
            var context = new SpecimenContext(fixture);
            var sut = CreateFreezeOnMatchCustomization(frozenType, Matching.ExactType);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = context.Resolve(frozenType);
            var requested = context.Resolve(requestedType);
            Assert.Equal(
                object.ReferenceEquals(frozen, requested),
                areSameSpecimen);
            // Teardown
        }

        [Theory]
        [InlineData(typeof(ConcreteType), typeof(ConcreteType), true)]
        [InlineData(typeof(ConcreteType), typeof(AbstractType), true)]
        [InlineData(typeof(string), typeof(object), true)]
        [InlineData(typeof(ConcreteType), typeof(object), true)]
        [InlineData(typeof(int), typeof(string), false)]
        public void FreezeByMatchingBaseTypeShouldReturnTheRightSpecimen(
            Type frozenType,
            Type requestedType,
            bool areSameSpecimen)
        {
            // Fixture setup
            var fixture = new Fixture();
            var context = new SpecimenContext(fixture);
            var sut = CreateFreezeOnMatchCustomization(frozenType, Matching.BaseType);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = context.Resolve(frozenType);
            var requested = context.Resolve(requestedType);
            Assert.Equal(
                object.ReferenceEquals(frozen, requested),
                areSameSpecimen);
            // Teardown
        }

        [Theory]
        [InlineData(typeof(ArrayList), typeof(ArrayList), true)]
        [InlineData(typeof(ArrayList), typeof(IEnumerable), true)]
        [InlineData(typeof(ArrayList), typeof(IList), true)]
        [InlineData(typeof(ArrayList), typeof(ICollection), true)]
        public void FreezeByMatchingImplementedInterfacesShouldReturnTheRightSpecimen(
            Type frozenType,
            Type requestedType,
            bool areSameSpecimen)
        {
            // Fixture setup
            var fixture = new Fixture();
            var context = new SpecimenContext(fixture);
            var sut = CreateFreezeOnMatchCustomization(frozenType, Matching.ImplementedInterfaces);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = context.Resolve(frozenType);
            var requested = context.Resolve(requestedType);
            Assert.Equal(
                object.ReferenceEquals(frozen, requested),
                areSameSpecimen);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingPropertyNameShouldReturnSameSpecimenForPropertiesOfSameNameAndType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var propertyName = "Property";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(frozenType, Matching.PropertyName, propertyName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<PropertyHolder<ConcreteType>>().Property;
            var requested = fixture.Create<PropertyHolder<ConcreteType>>().Property;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingPropertyNameShouldReturnSameSpecimenForPropertiesOfSameNameAndCompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var propertyName = "Property";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.PropertyName,
                propertyName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<PropertyHolder<ConcreteType>>().Property;
            var requested = fixture.Create<PropertyHolder<AbstractType>>().Property;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingPropertyNameShouldReturnDifferentSpecimensForPropertiesOfDifferentNamesAndSameType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var propertyName = "SomeOtherProperty";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.PropertyName,
                propertyName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<PropertyHolder<ConcreteType>>().Property;
            var requested = fixture.Create<PropertyHolder<ConcreteType>>().Property;
            Assert.NotSame(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingPropertyNameShouldThrowForPropertiesOfSameNameAndIncompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(string);
            var propertyName = "Property";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.PropertyName,
                propertyName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            Assert.Throws<ArgumentException>(() =>
                   fixture.Create<PropertyHolder<ConcreteType>>());
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingParameterNameShouldReturnSameSpecimenForParametersOfSameNameAndType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var parameterName = "parameter";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.ParameterName,
                parameterName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<SingleParameterType<ConcreteType>>().Parameter;
            var requested = fixture.Create<SingleParameterType<ConcreteType>>().Parameter;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingParameterNameShouldReturnSameSpecimenForParametersOfSameNameAndCompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var parameterName = "parameter";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.ParameterName,
                parameterName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<SingleParameterType<ConcreteType>>().Parameter;
            var requested = fixture.Create<SingleParameterType<AbstractType>>().Parameter;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingParameterNameShouldReturnDifferentSpecimensForParametersOfDifferentNamesAndSameType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var parameterName = "SomeOtherParameter";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.ParameterName,
                parameterName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<SingleParameterType<ConcreteType>>().Parameter;
            var requested = fixture.Create<SingleParameterType<ConcreteType>>().Parameter;
            Assert.NotSame(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingParameterNameShouldThrowForParametersOfSameNameAndIncompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(string);
            var parameterName = "parameter";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.ParameterName,
                parameterName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            Assert.Throws<ArgumentException>(() =>
                   fixture.Create<SingleParameterType<ConcreteType>>());
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingFieldNameShouldReturnSameSpecimenForFieldsOfSameNameAndType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var fieldName = "Field";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.FieldName,
                fieldName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<FieldHolder<ConcreteType>>().Field;
            var requested = fixture.Create<FieldHolder<ConcreteType>>().Field;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingFieldNameShouldReturnSameSpecimenForFieldsOfSameNameAndCompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var fieldName = "Field";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.FieldName,
                fieldName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<FieldHolder<ConcreteType>>().Field;
            var requested = fixture.Create<FieldHolder<AbstractType>>().Field;
            Assert.Same(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingFieldNameShouldReturnDifferentSpecimensForFieldsOfDifferentNamesAndSameType()
        {
            // Fixture setup
            var frozenType = typeof(ConcreteType);
            var fieldName = "SomeOtherField";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.FieldName,
                fieldName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            var frozen = fixture.Create<FieldHolder<ConcreteType>>().Field;
            var requested = fixture.Create<FieldHolder<ConcreteType>>().Field;
            Assert.NotSame(frozen, requested);
            // Teardown
        }

        [Fact]
        public void FreezeByMatchingFieldNameShouldThrowForFieldsOfSameNameAndIncompatibleTypes()
        {
            // Fixture setup
            var frozenType = typeof(string);
            var fieldName = "Field";
            var fixture = new Fixture();
            var sut = CreateFreezeOnMatchCustomization(
                frozenType,
                Matching.FieldName,
                fieldName);
            // Exercise system
            sut.Customize(fixture);
            // Verify outcome
            Assert.Throws<ArgumentException>(() =>
                   fixture.Create<FieldHolder<ConcreteType>>());
            // Teardown
        }

        private static ICustomization CreateFreezeOnMatchCustomization(
            Type frozenType,
            Matching matchBy = Matching.ExactType,
            string identifier = null)
        {
            var type = typeof(FreezeOnMatchCustomization<>).MakeGenericType(frozenType);
            return (ICustomization)Activator.CreateInstance(type, matchBy, identifier);
        }

    }
}
