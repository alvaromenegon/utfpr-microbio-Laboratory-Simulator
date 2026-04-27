using System;
using System.Linq;

public static class ExperimentMapper {
    public static ExperimentDefinition ToModel(ExperimentDefinitionDto dto) {
        if (dto == null) {
            return null;
        }

        return new ExperimentDefinition {
            id = dto.Id,
            title = dto.Title,
            createdAt = dto.CreatedAt,
            updatedAt = dto.UpdatedAt,
            userId = dto.UserId,
            experimentData = ToModel(dto.ExperimentData)
        };
    }

    public static ExperimentData ToModel(ExperimentDataDto dto) {
        if (dto == null) {
            return null;
        }

        return new ExperimentData {
            Description = dto.Description,
            Time = dto.Time,
            Objects = dto.Objects ?? Array.Empty<string>(),
            Steps = dto.Steps?.Select(ToModel).ToArray() ?? Array.Empty<Step>(),
            BackgroundProcesses = dto.BackgroundProcesses?.Select(ToModel).ToArray() ?? Array.Empty<BackgroundProcess>()
        };
    }

    public static Step ToModel(StepDto dto) {
        if (dto == null) {
            return null;
        }

        return new Step {
            Description = dto.Description,
            Score = dto.Score,
            interaction = dto.Interaction,
            InteractionTarget = dto.InteractionTarget,
            InteractionTargetSecondary = dto.InteractionTargetSecondary,
            Instruction = dto.Instruction,
            Value = dto.Value,
            Tolerance = dto.Tolerance,
            OnSuccessMessage = dto.OnSuccessMessage,
            OnFailureMessage = dto.OnFailureMessage,
            ActionOnFailure = dto.ActionOnFailure,
            OnFailureReturnToStep = dto.OnFailureReturnToStep,
            ActionOnSuccess = dto.ActionOnSuccess,
            OnSuccessStartBackgroundWorkId = dto.OnSuccessStartBackgroundWorkId,
        };
    }

    public static BackgroundProcess ToModel(BackgroundProcessDto dto) {
        if (dto == null) {
            return null;
        }

        return new BackgroundProcess {
            Id = dto.Id,
            Description = dto.Description,
            Duration = dto.Duration,
            Tolerance = dto.Tolerance,
            OnSuccessMessage = dto.OnSuccessMessage,
            OnFailureMessage = dto.OnFailureMessage,
            ActionOnFailure = dto.ActionOnFailure,
            OnFailureReturnToStep = dto.OnFailureReturnToStep,
            Score = dto.Score,
            ShouldCompleteBetweenSteps = dto.ShouldCompleteBetweenSteps,
        };
    }
}
