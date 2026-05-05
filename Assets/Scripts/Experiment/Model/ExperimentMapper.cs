using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

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
            Steps = dto.Steps?.Select(ToModel).ToList() ?? new List<Step>(),
            BackgroundProcesses = dto.BackgroundProcesses?.Select(ToModel).ToList() ?? new List<BackgroundProcess>(),
            ContinuousEffects = dto.ContinuousEffects?.Select(ToModel).ToList() ?? new List<ContinuousEffect>(),
            InstantEffects = dto.InstantEffects?.Select(ToModel).ToList() ?? new List<InstantEffect>(),
            TriggeredEffects = dto.TriggeredEffects?.Select(ToModel).ToList() ?? new List<TriggeredEffect>(),
            DerivedProperties = dto.DerivedProperties?.Select(ToModel).ToList() ?? new List<DerivedProperty>()
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
            Goals = dto.Goals?.Select(ToModel).ToList() ?? new List<When>()
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
        };
    }

    public static When ToModel(WhenDto dto) {
        if (dto == null) {
            return null;
        }
        return new When {
            Identifier = dto.Identifier,
            Property = dto.Property,
            Value = dto.Value,
            Comparison = Enum.TryParse(dto.Comparison, out ComparisonType result) ? result : ComparisonType.EQUAL
        };
    }

    public static EffectOperation ToModel(EffectOperationDto dto) {
        if (dto == null) {
            return null;
        }
        return new EffectOperation {
            TargetObject = dto.TargetObject,
            Property = dto.Property,
            Value = dto.Value
        };
    }

    public static DerivedProperty ToModel(DerivedPropertyDto dto) {
        if (dto == null) {
            return null;
        }
        return new DerivedProperty {
            Identifier = dto.Identifier,
            When = dto.When?.Select(ToModel).ToArray(),
            SetProperty = dto.SetProperty,
            Priority = dto.Priority
        };
    }

    public static TriggeredEffect ToModel(TriggeredEffectDto dto) {
        if (dto == null) {
            return null;
        }
        return new TriggeredEffect {
            Id = dto.Id,
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<When>(),
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>()
        };
    }

    public static InstantEffect ToModel(InstantEffectDto dto) {
        if (dto == null) {
            return null;
        }
        return new InstantEffect {
            InteractionType = Enum.TryParse(dto.InteractionType, out Interaction.InteractionType result) ? result : Interaction.InteractionType.USE,
            SourceObject = dto.SourceObject,
            TargetObject = dto.TargetObject,
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>(),
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<When>()
        };
    }

    public static ContinuousEffect ToModel(ContinuousEffectDto dto) {
        if (dto == null) {
            return null;
        }
        return new ContinuousEffect {
            InteractionType = Enum.TryParse(dto.InteractionType, out Interaction.InteractionType result) ? result : Interaction.InteractionType.USE,
            SourceObject = dto.SourceObject,
            TargetObject = dto.TargetObject,
            Property = dto.Property,
            Rate = dto.Rate,
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<When>()
        };
    }
}
