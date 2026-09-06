using System.ComponentModel.DataAnnotations;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.Customizations;

/// <summary>
/// End-to-end customization usage with [AutoDataSource], moved from the old Scenarios bucket.
/// </summary>
public class ScenarioTests
{
    [Test, AutoDataSource]
    public async Task WhenFirstParameterFrozen_SecondMatchesFirst([Frozen] Guid g1, Guid g2)
    {
        await Assert.That(g2).IsEqualTo(g1);
    }

    [Test, AutoDataSource]
    public async Task WhenSecondParameterFrozen_OnlySubsequentMatch(Guid g1, [Frozen] Guid g2, Guid g3)
    {
        await Assert.That(g2).IsNotEqualTo(g1);
        await Assert.That(g3).IsNotEqualTo(g1);

        await Assert.That(g3).IsEqualTo(g2);
    }

    [Test, AutoDataSource]
    public async Task WhenModestApplied_UsesModestConstructor([Modest] MultiUnorderedConstructorType p)
    {
        await Assert.That(string.IsNullOrEmpty(p.Text)).IsTrue();
        await Assert.That(p.Number).IsEqualTo(0);
    }

    [Test, AutoDataSource]
    public async Task WhenGreedyApplied_UsesGreedyConstructor([Greedy] MultiUnorderedConstructorType p)
    {
        await Assert.That(string.IsNullOrEmpty(p.Text)).IsFalse();
        await Assert.That(p.Number).IsNotEqualTo(0);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenAndGreedyOnSameParameter_BothApply(
        [Frozen][Greedy] MultiUnorderedConstructorType p1, MultiUnorderedConstructorType p2)
    {
        await Assert.That(p1).IsNotNull();
        await Assert.That(string.IsNullOrEmpty(p2.Text)).IsFalse();
        await Assert.That(p2.Number).IsNotEqualTo(0);
    }

    [Test, AutoDataSource]
    public async Task WhenGreedyAndFrozenOnSameParameter_BothApply(
        [Greedy][Frozen] MultiUnorderedConstructorType p1, MultiUnorderedConstructorType p2)
    {
        await Assert.That(p1).IsNotNull();
        await Assert.That(string.IsNullOrEmpty(p2.Text)).IsFalse();
        await Assert.That(p2.Number).IsNotEqualTo(0);
        await Assert.That(p2).IsSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenNoAutoPropertiesApplied_LeavesPropertiesUnset(
        [NoAutoProperties] PropertyHolder<object> ph1,
        [NoAutoProperties] PropertyHolder<string> ph2,
        [NoAutoProperties] PropertyHolder<int> ph3)
    {
        await Assert.That(ph1.Property).IsEqualTo(default(object));
        await Assert.That(ph2.Property).IsEqualTo(default(string));
        await Assert.That(ph3.Property).IsEqualTo(default(int));
    }

    [Test, AutoDataSource]
    public async Task WhenFavorArraysAndFrozen_InjectsArrayConstructorWithFrozenItems([Frozen] int[] numbers,
        [FavorArrays] ItemContainer<int> container)
    {
        await Assert.That(numbers.SequenceEqual(container.Items)).IsTrue();
    }

    [Test, AutoDataSource]
    public async Task WhenFavorEnumerablesApplied_UsesEnumerableConstructor(
        [FavorEnumerables] CompositeTypeWithOverloadedConstructors<int> numbers)
    {
        await Assert.That(numbers.Items).IsAssignableTo<IEnumerable<int>>();
        await Assert.That(numbers.Items).IsNotTypeOf<List<int>>();
        await Assert.That(numbers.Items).IsNotTypeOf<int[]>();
    }

    [Test, AutoDataSource]
    public async Task WhenFavorListsApplied_UsesListConstructor(
        [FavorLists] CompositeTypeWithOverloadedConstructors<string> strings)
    {
        await Assert.That(strings.Items).IsAssignableTo<List<string>>();
    }

    [Test, AutoDataSource]
    public async Task WhenFirstParameterFrozen_AssignsSameInstanceToSecond([Frozen] string p1,
        string p2)
    {
        await Assert.That(p2).IsEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByExactType_AssignsSameInstanceToSecond(
        [Frozen(Matching.ExactType)] ConcreteType p1,
        ConcreteType p2)
    {
        await Assert.That(p2).IsEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByExactType_DoesNotAssignToDifferentType(
        [Frozen(Matching.ExactType)] ConcreteType p1,
        object p2)
    {
        await Assert.That(p2).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByDirectBaseType_AssignsSameInstanceToSecond(
        [Frozen(Matching.DirectBaseType)] ConcreteType p1,
        AbstractType p2)
    {
        await Assert.That(p2).IsSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByDirectBaseType_DoesNotAssignToIndirectBaseType(
        [Frozen(Matching.DirectBaseType)] ConcreteType p1,
        object p2)
    {
        await Assert.That(p2).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByDirectBaseType_DoesNotAssignToSameType(
        [Frozen(Matching.DirectBaseType)] ConcreteType p1,
        ConcreteType p2)
    {
        await Assert.That(p2).IsNotEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByExactOrDirectBaseType_AssignsSameInstanceToSameType(
        [Frozen(Matching.ExactType | Matching.DirectBaseType)]
        ConcreteType p1,
        ConcreteType p2)
    {
        await Assert.That(p2).IsEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByInterface_AssignsSameInstanceToSecond(
        [Frozen(Matching.ImplementedInterfaces)]
        NoopInterfaceImplementer p1,
        IInterface p2)
    {
        await Assert.That(p2).IsSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByInterface_DoesNotAssignToNonInterfaceType(
        [Frozen(Matching.ImplementedInterfaces)]
        NoopInterfaceImplementer p1,
        object p2)
    {
        await Assert.That(p2).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByInterface_DoesNotAssignToSameType(
        [Frozen(Matching.ImplementedInterfaces)]
        NoopInterfaceImplementer p1,
        NoopInterfaceImplementer p2)
    {
        await Assert.That(p2).IsNotEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByDirectOrInterface_AssignsSameInstanceToSameType(
        [Frozen(Matching.ExactType | Matching.ImplementedInterfaces)]
        NoopInterfaceImplementer p1,
        NoopInterfaceImplementer p2)
    {
        await Assert.That(p2).IsEqualTo(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByParameterSameName_AssignsSameInstanceToSecond(
        [Frozen(Matching.ParameterName)] string parameter,
        SingleParameterType<object> p2)
    {
        await Assert.That(p2.Parameter).IsSameReferenceAs(parameter);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByParameterDifferentName_DoesNotAssignToSecond(
        [Frozen(Matching.ParameterName)] string p1,
        SingleParameterType<object> p2)
    {
        await Assert.That(p2.Parameter).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByParameterDifferentName_DoesNotAssignToSameType(
        [Frozen(Matching.ParameterName)] string p1,
        SingleParameterType<string> p2)
    {
        await Assert.That(p2.Parameter).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByPropertySameName_AssignsSameInstanceToSecond(
        [Frozen(Matching.PropertyName)] string property,
        PropertyHolder<object> p2)
    {
        await Assert.That(p2.Property).IsSameReferenceAs(property);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByPropertyDifferentName_DoesNotAssignToSecond(
        [Frozen(Matching.PropertyName)] string p1,
        PropertyHolder<object> p2)
    {
        await Assert.That(p2.Property).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByPropertyDifferentName_DoesNotAssignToSameType(
        [Frozen(Matching.PropertyName)] string p1,
        PropertyHolder<string> p2)
    {
        await Assert.That(p2.Property).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByFieldSameName_AssignsSameInstanceToSecond(
        [Frozen(Matching.FieldName)] string field,
        FieldHolder<object> p2)
    {
        await Assert.That(p2.Field).IsSameReferenceAs(field);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByFieldDifferentName_DoesNotAssignToSecond(
        [Frozen(Matching.FieldName)] string p1,
        FieldHolder<object> p2)
    {
        await Assert.That(p2.Field).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByFieldDifferentName_DoesNotAssignToSameType(
        [Frozen(Matching.FieldName)] string p1,
        FieldHolder<string> p2)
    {
        await Assert.That(p2.Field).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberSameName_AssignsSameInstanceToMatchingParameter(
        [Frozen(Matching.MemberName)] string parameter,
        SingleParameterType<object> p2)
    {
        await Assert.That(p2.Parameter).IsSameReferenceAs(parameter);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToParameter(
        [Frozen(Matching.MemberName)] string p1,
        SingleParameterType<object> p2)
    {
        await Assert.That(p2.Parameter).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToParameterOfSameType(
        [Frozen(Matching.MemberName)] string p1,
        SingleParameterType<string> p2)
    {
        await Assert.That(p2.Parameter).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberSameName_AssignsSameInstanceToMatchingProperty(
        [Frozen(Matching.MemberName)] string property,
        PropertyHolder<object> p2)
    {
        await Assert.That(p2.Property).IsSameReferenceAs(property);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToProperty(
        [Frozen(Matching.MemberName)] string p1,
        PropertyHolder<object> p2)
    {
        await Assert.That(p2.Property).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToPropertyOfSameType(
        [Frozen(Matching.MemberName)] string p1,
        PropertyHolder<string> p2)
    {
        await Assert.That(p2.Property).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberSameName_AssignsSameInstanceToMatchingField(
        [Frozen(Matching.MemberName)] string field,
        FieldHolder<object> p2)
    {
        await Assert.That(p2.Field).IsSameReferenceAs(field);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToField(
        [Frozen(Matching.MemberName)] string p1,
        FieldHolder<object> p2)
    {
        await Assert.That(p2.Field).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenByMemberDifferentName_DoesNotAssignToFieldOfSameType(
        [Frozen(Matching.MemberName)] string p1,
        FieldHolder<string> p2)
    {
        await Assert.That(p2.Field).IsNotSameReferenceAs(p1);
    }

    [Test, AutoDataSource]
    public async Task WhenFrozenWithStringLengthConstraint_CreatesConstrainedSpecimen(
        [Frozen, StringLength(3)] string p)
    {
        await Assert.That(p.Length == 3).IsTrue();
    }
}
