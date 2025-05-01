using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(Level))]
public class LevelShould
{
    [Fact]
    public void IsNeededChangeReturnTrueWhenCurrentPointsLessThanCurrentLevelNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(10);
        var regular = Level.Regular;

        // Act
        var actual = regular.IsNeededChange(points);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IsNeededChangeReturnTrueWhenCurrentPointsGreaterThanCurrentLevelAndGreaterThanNextLevelPointsNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(110);
        var fickle = Level.Fickle;

        // Act
        var actual = fickle.IsNeededChange(points);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IsNeededChangeReturnFalseWhenCurrentLevelIsMaximum()
    {
        // Arrange
        var points = LoyaltyPoints.Create(310);
        var frequent = Level.Frequent;

        // Act
        var actual = frequent.IsNeededChange(points);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void
        GetNewLevelForChangingReturnNextLevelForCurrentLevelIfPointsGreaterThanCurrentLevelAndNextLevelPointsNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(110);
        var fickle = Level.Fickle;

        // Act
        var actual = fickle.GetNewLevelForChanging(points);

        // Assert
        Assert.Equal(Level.Regular, actual);
    }

    [Fact]
    public void
        GetNewLevelForChangingReturnPreviousLevelForCurrentLevelIfPointsLessThanCurrentLevel()
    {
        // Arrange
        var points = LoyaltyPoints.Create(90);
        var regular = Level.Regular;

        // Act
        var actual = regular.GetNewLevelForChanging(points);

        // Assert
        Assert.Equal(Level.Fickle, actual);
    }

    [Fact]
    public void GetNewLevelForChangingThrowDomainRulesViolationExceptionIfChangingNotNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(50);
        var fickle = Level.Fickle;

        // Act
        void Act()
        {
            fickle.GetNewLevelForChanging(points);
        }

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void FromNameReturnCorrectLevel()
    {
        // Arrange
        var name = Level.Fickle.Name;

        // Act
        var actual = Level.FromName(name);

        // Assert
        Assert.Equal(Level.Fickle, actual);
    }

    [Fact]
    public void FromNameThrowValueOutOfRangeExceptionIfNameIsUnknown()
    {
        // Arrange
        var invalidName = "some-level";

        // Act
        void Act()
        {
            Level.FromName(invalidName);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void FromIdReturnCorrectLevel()
    {
        // Arrange
        var id = Level.Fickle.Id;

        // Act
        var actual = Level.FromId(id);

        // Assert
        Assert.Equal(Level.Fickle, actual);
    }

    [Fact]
    public void FromIdThrowValueOutOfRangeExceptionIfIdIsUnknown()
    {
        // Arrange
        var invalidId = 42;

        // Act
        void Act()
        {
            Level.FromId(invalidId);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void EqualOperatorReturnTrueForEqualLevels()
    {
        // Arrange
        var level1 = Level.Fickle;
        var level2 = Level.Fickle;

        // Act
        var actual = level1 == level2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnTrueForDifferentLevels()
    {
        // Arrange
        var level1 = Level.Fickle;
        var level2 = Level.Regular;

        // Act
        var actual = level1 != level2;

        // Assert
        Assert.True(actual);
    }
}