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
            Effects = dto.Effects?.Select(ToModel).ToList() ?? new List<EffectDefinition>(),
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
            TargetRole = Enum.TryParse(dto.TargetRole, out RoleType result) ? result : (RoleType?)null,
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

    public static EffectDefinition ToModel(EffectDefinitionDto dto) {
        if (dto == null) {
            return null;
        }
        if (!Enum.TryParse(dto.ExecutionType, out EffectExecutionType executionType)) {
            throw new ArgumentException($"Invalid execution type: {dto.ExecutionType}");
        }

        return new EffectDefinition {
            Type = Enum.TryParse(dto.Type, out Interaction.InteractionType result) ? result : Interaction.InteractionType.USE,
            ExecutionType = executionType,
            Property = dto.Property,
            Rate = dto.Rate,
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>(),
            Roles = dto.Roles?.ToDictionary(kv => ResolveRoleType(kv.Key), kv => kv.Value) ?? new Dictionary<RoleType, string>(),
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<When>()
        };
    }

    public static RoleType ResolveRoleType(string role) {
        return Enum.TryParse(role, out RoleType result) ? result : RoleType.ACTOR;
    }
}
