using Domain.RentAggregate;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using Domain.SharedKernel.Exceptions.ArgumentException;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.RentAggregate;

[TestSubject(typeof(Status))]
public class StatusShould
{
    [Fact]
    public void FromNameReturnCorrectStatus()
    {
        // Arrange
        var name = Status.InProgress.Name;

        // Act
        var actual = Status.FromName(name);

        // Assert
        Assert.Equal(Status.InProgress, actual);
    }

    [Fact]
    public void FromNameThrowValueOutOfRangeExceptionIfStatusNameIsNotSupported()
    {
        // Arrange
        var name = "unsupported";

        // Act
        void Act()
        {
            Status.FromName(name);
        }

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void FromIdReturnCorrectStatus()
    {
        // Arrange
        var id = Status.InProgress.Id;

        // Act
        var actual = Status.FromId(id);

        // Assert
        Assert.Equal(Status.InProgress, actual);
    }

    [Fact]
    public void FromIdThrowValueOutOfRangeExceptionIfStatusIdIsNotSupported()
    {
        // Arrange
        var id = 434;

        // Act
        void Act()
        {
            Status.FromId(id);
        }

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void EqualOperatorShouldReturnTrueIfStatusesIsEqual()
    {
        // Arrange
        var status1 = Status.InProgress;
        var status2 = Status.InProgress;

        // Act
        var actual = status1 == status2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorShouldReturnTrueIfStatusesIsDifferent()
    {
        // Arrange
        var status1 = Status.InProgress;
        var status2 = Status.Completed;

        // Act
        var actual = status1 != status2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void CanBeChangedToThisStatusThrowAlreadyHaveThisStateExceptionIfPotentialStatusIsEqualCurrent()
    {
        // Arrange
        var inProcess = Status.InProgress;

        // Act
        void Act()
        {
            inProcess.CanBeChangedToThisStatus(Status.InProgress);
        }

        // Assert
        Assert.Throws<AlreadyHaveThisStateException>(Act);
    }

    [Fact]
    public void CanBeChangedToThisStatusThrowValueIsRequiredExceptionIfPotentialStatusIsNull()
    {
        // Arrange
        var inProcess = Status.InProgress;

        // Act
        void Act()
        {
            inProcess.CanBeChangedToThisStatus(null!);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void CanBeChangedToThisStatusReturnFalseForThisStatuses()
    {
        // Arrange

        // Act
        var completedToInProgress = Status.Completed.CanBeChangedToThisStatus(Status.InProgress);


        // Assert
        Assert.False(completedToInProgress);
    }

    [Fact]
    public void CanBeChangedToThisStatusReturnTrueForThisStatuses()
    {
        // Arrange

        // Act
        var inProgressToCompleted = Status.InProgress.CanBeChangedToThisStatus(Status.Completed);

        // Assert
        Assert.True(inProgressToCompleted);
    }
}