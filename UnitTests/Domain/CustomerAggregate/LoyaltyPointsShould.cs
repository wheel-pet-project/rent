using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(LoyaltyPoints))]
public class LoyaltyPointsShould
{
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange
        var startPoints = 10;

        // Act
        var actual = LoyaltyPoints.Create(startPoints);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(startPoints, actual.Value);
    }

    [Fact]
    public void ThrowValueOutOfRangeExceptionWhenStartPointsLessThanLowestLevel()
    {
        // Arrange
        var startPoints = -1;

        // Act
        void Act()
        {
            LoyaltyPoints.Create(startPoints);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void ThrowValueOutOfRangeExceptionWhenStartPointsGreaterThanHighestLevel()
    {
        // Arrange
        var startPoints = 401;

        // Act
        void Act()
        {
            LoyaltyPoints.Create(startPoints);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void CreateFromTripsReturnHighestLevelFo40RentsPerMount()
    {
        // Arrange
        var rents = 40;

        // Act
        var actual = LoyaltyPoints.CreateFromRents(rents);

        // Assert
        Assert.Equal(400, actual.Value);
    }

    [Fact]
    public void CreateFromTripsReturnLowestLevel0Rents()
    {
        // Arrange
        var rents = 0;

        // Act
        var actual = LoyaltyPoints.CreateFromRents(rents);

        // Assert
        Assert.Equal(1, actual.Value);
    }

    [Fact]
    public void EqualOperatorReturnsTrueForEqualPoints()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(10);
        var points2 = LoyaltyPoints.Create(10);

        // Act
        var actual = points1 == points2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnsTrueDifferentPoints()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(10);
        var points2 = LoyaltyPoints.Create(88);

        // Act
        var actual = points1 != points2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void GreaterThanOperatorReturnsTrueForLessPointsQuantity()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(10);
        var points2 = LoyaltyPoints.Create(5);

        // Act
        var actual = points1 > points2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void LessThanOperatorReturnsTrueForGreaterPointsQuantity()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(5);
        var points2 = LoyaltyPoints.Create(10);

        // Act
        var actual = points1 < points2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void LessThanOperatorReturnsFalseForOneOfNull()
    {
        // Arrange
        LoyaltyPoints points1 = null!;
        var points2 = LoyaltyPoints.Create(10);

        // Act
        var actual = points1 < points2;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GreaterThanOperatorReturnsFalseForOneOfNull()
    {
        // Arrange
        LoyaltyPoints points1 = null!;
        var points2 = LoyaltyPoints.Create(10);

        // Act
        var actual = points1 > points2;

        // Assert
        Assert.False(actual);
    }
}